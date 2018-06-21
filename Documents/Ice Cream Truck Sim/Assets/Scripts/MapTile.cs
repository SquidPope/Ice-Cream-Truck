using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour 
{
	Vector2 mapPos;

	public Vector2 MapPos
	{
		get { return mapPos; }
		set
		{
			mapPos = value;
			gameObject.name = "tile " + mapPos;
		}
	}

	public Vector3 WorldPos
	{
		get { return transform.position; }
		set { transform.position = value; }
	}
}
