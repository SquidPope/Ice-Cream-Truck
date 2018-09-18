﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class TruckController : MonoBehaviour
{

	//ToDo: when we hit something (building, etc.) set velocity to 0 so it's easier to back up
	new Rigidbody rigidbody;
	AudioSource audioSource;

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

	public float Velocity
	{
		get { return vel; }
	}

	public float Acceleration
	{
		get { return acceleration; }
	}

	void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.Pause();
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Follower")
		{
			//ToDo: something, idk
			return;
		}
		else if (other.gameObject.tag == "Tile")
		{
			currentTile = other.gameObject;
		}
		else
		{
			//We hit a building or something.
			//ToDo: As long as we are still hitting a building or something, don't accelerate into it.
			vel = 0;
			rigidbody.velocity = Vector3.zero;
		}
		
		MapController.Instance.UpdateMap();
	}

	void FixedUpdate() 
	{
		//ToDo: slight deceleration if no button is down?
		if (Input.GetKey(KeyCode.W))
		{
			if (vel < maxSpeed)
				vel += acceleration;
		}
		else if (Input.GetKey(KeyCode.S)) //if s slow down (does that work if w is pressed too?)
		{
			//ToDo: Set a lower speed limit on backing up.
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
			else
			{
				//Reverse
				vel -= acceleration;
				rigidbody.velocity = transform.forward * vel;
			}
		}
		else
		{
			//Slow to a gradual stop if we're not hitting the gas.
			if (vel > 0)
				vel -= idleDecrease;
			else if (vel < 0)
				vel += idleDecrease;

			//if we idled below 0 (idleLimit?) set to 0
			//if (vel < 0)
			//	vel = 0;
		}

		if (vel != 0) //ToDo: reverse?
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

		if (Input.GetMouseButtonUp(0))
		{
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
			}
			else
			{
				audioSource.UnPause();
			}
		}
	}
}
