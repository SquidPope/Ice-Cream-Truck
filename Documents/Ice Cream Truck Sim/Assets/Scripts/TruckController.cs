using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class TruckController : MonoBehaviour
{
	new Rigidbody rigidbody;
	AudioSource audioSource;

	float vel = 0;
	float acceleration = 0.1f; //ToDo: acceleration should increase the longer the same button (w or s) is held, up to a max.
	float maxSpeed = 20f;
	float turnRate = 2f;
	float idleDecrease = 0.01f;

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

	public bool IsMusicPlaying
	{
		get { return audioSource.isPlaying; }
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
		//If we aren't playing, don't even
		if (GameController.Instance.State != GameState.Playing)
			return;

		//ToDo: slight deceleration if no button is down?
		if (Input.GetKey(KeyCode.W))
		{
			if (vel < maxSpeed)
				vel += acceleration;

			//ToDo: Slow down if we have been moving backwards.
		}
		else if (Input.GetKey(KeyCode.S)) //if s slow down (does that work if w is pressed too?)
		{
			//ToDo: Set a lower speed limit on backing up.
			if (vel > acceleration)
			{
				//Slow down.
				vel -= acceleration;
			}
			else if (vel > 0)
			{
				//If we are really close to a full stop, just stop.
				vel = 0;
				rigidbody.velocity = Vector3.zero;
			}
			else if (vel > -acceleration)
			{
				//If we are really close to a full stop, just stop.
				vel = 0;
				rigidbody.velocity = Vector3.zero;
			}
			else
			{
				//Reverse
				if (vel > -maxSpeed) //ToDo: have a different max speed for reverse?
				{
					vel -= acceleration;
				}
			}
		}
		else
		{
			//Slow to a gradual stop if we're not hitting the gas.
			if (vel > 0)
				vel -= idleDecrease;
			else if (vel < 0)
				vel += idleDecrease;

			if (vel < acceleration && vel > -acceleration)
			{
				//If we're really close to a full stop, just stop.
				vel = 0;
			}
		}

		if (vel != 0)
		{
			if (Input.GetKey(KeyCode.A))
			{
				//move forward or backward and turn left
				transform.Rotate(new Vector3(0f, -turnRate, 0f), Space.Self);
			}
			else if (Input.GetKey(KeyCode.D))
			{
				//move forward or backward and turn right
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
