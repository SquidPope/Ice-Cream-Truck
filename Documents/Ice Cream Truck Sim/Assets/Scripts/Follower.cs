using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : MonoBehaviour
{
	new Rigidbody rigidbody;

	float followDist = 1f;

	void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		followDist = TruckController.Instance.transform.localScale.z * 3f; //Always stay three car lengths behind the car in front of you!
	}
	
	void Update()
	{
		//ToDo: AI to path around obstacles? or just despawn?

		float vel = TruckController.Instance.Velocity;
		gameObject.transform.LookAt(TruckController.Instance.transform);

		//ToDo: Slow to stop, and then reverse when player gets too close
		if (Vector3.Distance(TruckController.Instance.transform.position, transform.position) <= followDist)
		{
			rigidbody.velocity = -transform.forward * 10f;//(TruckController.Instance.Acceleration * 10f);
		}
		else
		{
			rigidbody.velocity = transform.forward * vel;
		}	
	}
}
