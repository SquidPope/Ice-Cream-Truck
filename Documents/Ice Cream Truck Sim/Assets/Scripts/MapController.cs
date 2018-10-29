using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public List<GameObject> prefabs;
	public GameObject prefabToCut;

	public int mapWidth = 7;
	public int mapHeight = 7;
	List<MapTile> mapTiles; //object pool (should we use an array? size is static.)

	float offsetX = 1f;
	float offsetZ = 1f;

	static MapController instance;
	public static MapController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("MapController");
				instance = go.GetComponent<MapController>();
			}

			return instance;
		}
	}
	
	void Awake()
	{
		mapTiles = new List<MapTile>();
		GenerateMap();

		//Move the truck to just above the center of the map.
		Vector2 mapCenter = GetMapCenter();
		Vector3 truckPos = mapTiles.Find(x => x.MapPos == mapCenter).WorldPos;
		truckPos.y += 2f;
		TruckController.Instance.WorldPos = truckPos;

		//If the map size is really small, remove a prefab with walls in it so the player doesn't get stuck.
		if (XMLController.Instance.Options.mapSize <= 10)
		{
			if (prefabToCut != null && prefabs.Contains(prefabToCut))
				prefabs.Remove(prefabToCut);
		}
	}

	void GenerateMap()
	{
		int size = XMLController.Instance.GetMapSize();
		if (size > 1)
		{
			mapHeight = size;
			mapWidth = size;
		}
		else
		{
			//If the saved map size is invalid, use the default.
			if (mapHeight < 2)
				mapHeight = 2;

			if (mapWidth  < 2)
				mapWidth = 2;
		}

		//Should be the same for all tiles
		Renderer prefabRenderer = prefabs[0].GetComponent<Renderer>();
		offsetX = prefabRenderer.bounds.extents.x * 2f;
		offsetZ = prefabRenderer.bounds.extents.z * 2f;

		float tileX = 0f;
		float tileZ = 0f;

		//check if prefabs are empty (create a default one?)
		if (prefabs.Count <= 0)
		{
			prefabs.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
		}

		GameObject prefab = prefabs[0];

		//create a square map
		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				//ToDo: should we place this after instantiating so the first tile is consistant?
				if (prefabs.Count > 1)
				{
					int rand = Random.Range(0, prefabs.Count);
					prefab = prefabs[rand];
				}

				GameObject tile = GameObject.Instantiate(prefab, new Vector3(tileX, 0f, tileZ), Quaternion.identity);

				int flip = Random.Range(0, 2);
				if (flip == 1)
				{
					tile.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
				}

				MapTile mapTile = tile.AddComponent<MapTile>();
				mapTile.MapPos = new Vector2(i, j);
				mapTiles.Add(mapTile);

				//Change tilePos to next tile
				tileX += offsetX;
			}
			
			tileZ += offsetZ;
			tileX = 0f;
			//ToDo: player should spawn at center tile.
		}
	}

	public void UpdateMap()
	{
		MapTile truckTile = TruckController.Instance.CurrentTile;
		if (truckTile == null || !mapTiles.Contains(truckTile))
		{
			Debug.LogError("Truck Tile is null or does not exist in map");
			return;
		}
		
		//find each tile that is too far away
		//if x == 4, row 0 should move "up"
		//if y == 2, column 4 should move "left"

		//find center tile, compare x and y to truckTile
		Vector2 mapCenter = GetMapCenter();

		if (truckTile.MapPos.x > mapCenter.x)
		{
			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.x == mapWidth - 1); //Just find the first one, they all have the same x.
			float nextTileZ = edge.WorldPos.z;
			nextTileZ += offsetZ;

			for (int i = 0; i < mapTiles.Count; i++)
			{
				Vector2 mapPos = mapTiles[i].MapPos;
				mapTiles[i].MapPos = new Vector2(mapPos.x - 1, mapPos.y);
			}

			List<MapTile> rowToMove = mapTiles.FindAll(x => x.MapPos.x == -1);
			foreach (MapTile m in rowToMove)
			{
				//keep y(z in world space?) the same, move x to map pos of x the highest world x + x offset
				m.WorldPos =  new Vector3(m.WorldPos.x, 0f, nextTileZ);
				m.MapPos = new Vector2(mapWidth - 1, m.MapPos.y);
				m.RandomizeMessages();
			}
		}
		else if (truckTile.MapPos.x < mapCenter.x)
		{
			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.x == 0); //Just find the first one, they all have the same x.
			float nextTileZ = edge.WorldPos.z;
			nextTileZ -= offsetZ;
			
			for (int i = 0; i < mapTiles.Count; i++)
			{
				Vector2 mapPos = mapTiles[i].MapPos;
				mapTiles[i].MapPos = new Vector2(mapPos.x + 1, mapPos.y);
			}

			List<MapTile> rowToMove = mapTiles.FindAll(x => x.MapPos.x == mapWidth);
			foreach (MapTile m in rowToMove)
			{
				//keep y(z in world space?) the same, move x to map pos of x the highest world x + x offset
				m.WorldPos =  new Vector3(m.WorldPos.x, 0f, nextTileZ);
				m.MapPos = new Vector2(0, m.MapPos.y);
				m.RandomizeMessages();
			}
		}

		if (truckTile.MapPos.y > mapCenter.y)
		{
			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.y == mapHeight - 1); //Just find the first one, they all have the same x.
			float nextTileX = edge.WorldPos.x;
			nextTileX += offsetX;

			for (int i = 0; i < mapTiles.Count; i++)
			{
				Vector2 mapPos = mapTiles[i].MapPos;
				mapTiles[i].MapPos = new Vector2(mapPos.x, mapPos.y - 1);
			}

			List<MapTile> columnToMove = mapTiles.FindAll(x => x.MapPos.y == -1);
			foreach (MapTile m in columnToMove)
			{
				//keep y(z in world space?) the same, move x to map pos of x the highest world x + x offset
				m.WorldPos =  new Vector3(nextTileX, 0f, m.WorldPos.z);
				m.MapPos = new Vector2(m.MapPos.x, mapHeight - 1);
				m.RandomizeMessages();
			}
		}
		else if (truckTile.MapPos.y < mapCenter.y)
		{
			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.y == 0); //Just find the first one, they all have the same x.
			float nextTileX = edge.WorldPos.x;
			nextTileX -= offsetX;

			for (int i = 0; i < mapTiles.Count; i++)
			{
				Vector2 mapPos = mapTiles[i].MapPos;
				mapTiles[i].MapPos = new Vector2(mapPos.x, mapPos.y + 1);
			}

			List<MapTile> columnToMove = mapTiles.FindAll(x => x.MapPos.y == mapHeight);
			foreach (MapTile m in columnToMove)
			{
				//keep y(z in world space?) the same, move x to map pos of x the highest world x + x offset
				m.WorldPos =  new Vector3(nextTileX, 0f, m.WorldPos.z);
				m.MapPos = new Vector2(m.MapPos.x, 0);
				m.RandomizeMessages();
			}
		}

		//we should always be within 1 of the center (3 in this case) because we update every tile move(?)
		//so, x changed means row 0 or max should move, y change means column 0 or max should move 

		//re-arrange tiles' addresses (we always want x and y to be 0 - max)
		//it would be cheaper to have an object pool list in whateverthesquid order, and each TILE has it's own x and y map address, but then we can't get them by address (use Find(adress))

		//UpdateTile(currentTile)

		//ideally something like:
		//0 0 0 0 0 row4
		//0 0 0 0 0 row3
		//0 0 1 0 0 row2
		//0 0 0 0 0 row1
		//0 0 0 0 0 row0
		//where 1 is player

		//ToDo: do we prevent diagonal movement (fences on tile?) or do we figure that player still won't see what's happening in near corner (since it's more than 1 tile away still?)
		//1 moves "up"
		//bottom corners (or whole row?) recycled to make tiles in front
		//player should be able to see 2 rows ahead (geometry of model to prevent farther? haze? enourmous tiles?)
	}

	Vector2 GetMapCenter()
	{
		Vector2 center = new Vector2();

		//Get the center tile of the map (or as close as possible if the width and/or height are even).
		if (mapWidth % 2 > 0)
		{
			center.x = mapWidth / 2 + 1;
		}
		else
		{
			center.x = mapWidth / 2;
		}

		if (mapHeight % 2 > 0)
		{
			center.y = mapHeight / 2 + 1;
		}
		else
		{
			center.y = mapHeight / 2;
		}

		//Move the ids back one because they start at 0.
		center.x -= 1;
		center.y -= 1;

		return center;
	}

	public Vector3 GetCenterPos()
	{
		Vector3 pos = Vector3.zero;

		Vector2 tileID = GetMapCenter();
		MapTile tile = mapTiles.Find(x => x.MapPos == tileID);
		
		if (tile != null)
			pos = tile.WorldPos;

		return pos;
	}
}
