using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour 
{
	Vector2 mapPos;

	Messages[] messageComponents;
	bool hasMessages = false;

	public Vector3 worldPos;

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

	void Start()
	{
		messageComponents = gameObject.transform.GetComponentsInChildren<Messages>();
		if (messageComponents.Length > 0)
			hasMessages = true;

		worldPos = transform.position;
	}

	public void RandomizeMessages()
	{
		if (hasMessages)
		{
			foreach (Messages m in messageComponents)
				m.RandomizeMessage();
		}
	}
}
