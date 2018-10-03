using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour 
{
	public GameObject prefab;
	public float distance;

	GameObject spawn;
	Follower currentFollower = null;

	void Start()
	{
		Transform truck = TruckController.Instance.gameObject.transform;
		spawn = new GameObject();
		spawn.name = "Follower Spawn";
		spawn.transform.position = truck.position;
		spawn.transform.SetParent(truck);
		spawn.transform.position -= truck.forward * distance;
	}
	
	void Update() 
	{
		//ToDo: Spawn at interval
		if (Input.GetKeyDown(KeyCode.K) && !TruckController.Instance.IsMusicPlaying)
		{
			if (currentFollower == null) //ToDo: Do we want multiple followers?
				currentFollower = GameObject.Instantiate(prefab, spawn.transform.position, spawn.transform.rotation).GetComponent<Follower>();
		}	
	}
}
