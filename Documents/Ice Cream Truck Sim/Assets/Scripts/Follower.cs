using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : MonoBehaviour
{
	new Rigidbody rigidbody;

	void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	void Update()
	{
		//move to a certain distance behind TruckController
		//gameObject.transform.rotation = TruckController.Instance.Rotation;

		float vel = TruckController.Instance.Velocity;
		gameObject.transform.LookAt(TruckController.Instance.transform);

		rigidbody.velocity = transform.forward * vel;
	}
}
