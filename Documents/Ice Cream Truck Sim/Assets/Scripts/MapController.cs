using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public List<GameObject> prefabs;

	public int mapWidth = 7;
	public int mapHeight = 7;
	List<MapTile> mapTiles; //object pool (should we use an array? size is static.)

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
	
	void Start()
	{
		mapTiles = new List<MapTile>();
		GenerateMap();

		//Move the truck to just above the center of the map.
		Vector2 mapCenter = GetMapCenter();
		Vector3 truckPos = mapTiles.Find(x => x.MapPos == mapCenter).WorldPos;
		truckPos.y += 2f;
		TruckController.Instance.WorldPos = truckPos;
	}

	void GenerateMap()
	{
		if (mapHeight < 2)
			mapHeight = 2;

		if (mapWidth  < 2)
			mapWidth = 2;

		float tileX = 0f;
		float tileZ = 0f;

		//check if prefabs are empty (create a default one?)
		if (prefabs.Count <= 0)
		{
			prefabs.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
		}

		GameObject prefab = prefabs[0];
		
		float offsetX = prefab.transform.localScale.x;
		float offsetZ = prefab.transform.localScale.z;

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

					offsetX = prefab.transform.localScale.x;
					offsetZ = prefab.transform.localScale.z;
				}
				
				GameObject tile = GameObject.Instantiate(prefab, new Vector3(tileX, 0f, tileZ), Quaternion.identity);
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
		
		//find each tile that is too far away ??
		//if x == 4, row 0 should move "up"
		//if y == 2, column 4 should move "left"

		//find center tile, compare x and y to truckTile
		Vector2 mapCenter = GetMapCenter();

		//ToDo: Figure out why moving a tile will always mean the truck is offset from the center by x AND y

		List<MapTile> column = mapTiles.FindAll(x => x.MapPos.x == truckTile.MapPos.x);
		if (truckTile.MapPos.x > mapCenter.x)
		{
			Debug.Log("truck is above?! center");

			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.x == mapWidth - 1); //Just find the first one, they all have the same x.
			float nextTileZ = edge.WorldPos.z;
			nextTileZ += edge.transform.localScale.z;

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
			}
		}
		else if (truckTile.MapPos.x < mapCenter.x)
		{
			Debug.Log("truck is below?! center");

			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.x == 0); //Just find the first one, they all have the same x.
			float nextTileZ = edge.WorldPos.z;
			nextTileZ -= edge.transform.localScale.z;
			
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
			}
		}

		if (truckTile.MapPos.y > mapCenter.y)
		{
			Debug.Log("truck is right of?! center");

			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.y == mapHeight - 1); //Just find the first one, they all have the same x.
			float nextTileX = edge.WorldPos.x;
			nextTileX += edge.transform.localScale.x;

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
			}
		}
		else if (truckTile.MapPos.y < mapCenter.y)
		{
			Debug.Log("truck is left of?! center");

			//Find the current right edge and calculate where the tile should move to.
			MapTile edge = mapTiles.Find(x => x.MapPos.y == 0); //Just find the first one, they all have the same x.
			float nextTileX = edge.WorldPos.x;
			nextTileX -= edge.transform.localScale.x;

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
}
