using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TruckController : MonoBehaviour
{

	new Rigidbody rigidbody;
	float vel = 0;
	float acceleration = 0.01f;
	float maxSpeed = 20f;
	float turnRate = 2f;
	float idleDecrease = 0.001f;

	GameObject currentTile;

	static TruckController instance;
	public static TruckController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("Truck");
				instance = go.GetComponent<TruckController>();
			}

			return instance;
		}
	}

	public Vector3 WorldPos
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public MapTile CurrentTile
	{
		get { return currentTile.GetComponent<MapTile>(); }
	}

	void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();	
	}

	void OnCollisionEnter(Collision other)
	{
		//ToDo: make sure walls don't count for this when we have them
		currentTile = other.gameObject;
		MapController.Instance.UpdateMap();
	}

	void Update() 
	{
		//ToDo: slight deceleration if no button is down?
		if (Input.GetKey(KeyCode.W))
		{
			if (vel < maxSpeed)
				vel += acceleration;
		}
		else if (Input.GetKey(KeyCode.S)) //if s slow down (does that work if w is pressed too?)
		{
			//if force is < 10, set to 0?
			//rigidbody.AddForce(-10f, 0f, 0f);
			if (vel > acceleration)
			{
				vel -= acceleration;
				rigidbody.velocity = transform.forward * vel;
			}
			else if (vel > 0)
			{
				vel = 0;
				rigidbody.velocity = Vector3.zero;
			}	
		}
		else
		{
			//Slow to a gradual stop if we're not hitting the gas.
			if (vel > 0)
				vel -= idleDecrease;

			//if we idled below 0 (idleLimit?) set to 0
			if (vel < 0)
				vel = 0;
		}

		if (vel > 0) //ToDo: reverse?
		{
			if (Input.GetKey(KeyCode.A))
			{
				//move forward and turn left
				transform.Rotate(new Vector3(0f, -turnRate, 0f), Space.Self);
			}
			else if (Input.GetKey(KeyCode.D))
			{
				//move forward and turn right
				transform.Rotate(new Vector3(0f, turnRate, 0f));
			}

			rigidbody.velocity = transform.forward * vel;
		}
	}
}
