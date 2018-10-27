using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour 
{
	//ToDo: Followers should be more likely to spawn/harder to get rid of when game timer is low.
	public GameObject prefab;
	public float distance;

	GameObject spawn;
	Follower currentFollower = null;

	float spawnInterval = 15f;
	float spawnTimer = 0f;
	float despawnLimit = 30f;

	static FollowerManager instance;
	public static FollowerManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("FollowerManager");
				instance = go.GetComponent<FollowerManager>();
			}

			return instance;
		}
	}

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
		if (currentFollower == null) //ToDo: Do we ever want multiple followers?
			spawnTimer += Time.deltaTime;
		else if (TruckController.Instance.IsMusicPlaying && GameController.Instance.timeLeft > despawnLimit)
			currentFollower.DestroySelf(); //Does it need to be offscreen? or can we just do a poof?

		if (spawnTimer >= spawnInterval)
		{
			if (IsPointOffScreen(spawn.transform.position) || ThirdPersonCamera.instance.CheckCameraPoints(spawn.transform.position, Camera.main.transform.position) != -1) //The Truck blocking the camera's view of the spawn point will not allow the Follower to spawn.
			{
				if (!TruckController.Instance.IsMusicPlaying)
				{
					currentFollower = GameObject.Instantiate(prefab, spawn.transform.position, spawn.transform.rotation).GetComponent<Follower>();
					spawnTimer = 0f;
				}
			}
		}

		//ToDo: Despawn for some reason (Music on? timer?)		
	}

	bool IsPointOffScreen(Vector3 worldPos)
	{
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
		bool isOffScreen = true;

		if (screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height)
		{
			isOffScreen = false;
		}

		return isOffScreen;
	}

	public void RemoveFollower()
	{
		currentFollower = null;
	}
}
