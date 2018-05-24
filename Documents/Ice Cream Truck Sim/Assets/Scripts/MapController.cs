using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public List<GameObject> prefabs;

	int mapWidth = 5;
	int mapHeight = 5;
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
				tile.name = "tile " + i + " - " + j;
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

		if (truckTile.MapPos.x > mapCenter.x)
		{
			Debug.Log("truck is right of?! center");
		}
		else if (truckTile.MapPos.x < mapCenter.x)
		{
			Debug.Log("truck is left of?! center");
		}

		if (truckTile.MapPos.y > mapCenter.y)
		{
			Debug.Log("truck is above?! center");
		}
		else if (truckTile.MapPos.y < mapCenter.y)
		{
			Debug.Log("truck is below?! center");
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

	void UpdateTile(MapTile tile, Vector3 position)
	{
		//ToDo: randomize the tile's prefab
		//Move the tile to the correct new position (based on direction player is moving in ??)
		tile.WorldPos = position;
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
