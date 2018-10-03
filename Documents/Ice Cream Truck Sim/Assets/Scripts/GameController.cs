using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public int score = 0;
	public float timeLimit; //In seconds
	public float timeLeft;
	public float iceCreamDelay; //Also in seconds
	public float iceCreamProgress = 0f;
	bool truckInZone = false;
	bool hasSoldIceCream = false;

	static GameController instance;
	public static GameController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("GameController");
				instance = go.GetComponent<GameController>();
			}

			return instance;
		}
	}

	public bool TruckInZone
	{
		get { return truckInZone; }
		set
		{
			if (!value)
			{
				hasSoldIceCream = false;
			}
			
			truckInZone = value;
		}
	}

	void Awake() 
	{
		ThirdPersonCamera.UseExistingOrCreateMainCamera();
		UIController.Instance.UpdateUI();
		timeLeft = timeLimit;
	}
	
	// Update is called once per frame
	void Update() 
	{
		//if we aren't paused
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0)
		{
			//Game Over
		}

		if (truckInZone)
		{
			if (TruckController.Instance.Velocity == 0)
			{
				//Display UI "Press Space to sell ice cream"
				if (Input.GetKey(KeyCode.Space))
				{
					if (!hasSoldIceCream)
					{
						iceCreamProgress += Time.deltaTime;

						//Fill a bar slowly, give point when full.
						if (iceCreamProgress >= iceCreamDelay)
						{
							score++;
							hasSoldIceCream = true;
							//ToDo:Make iceCreamDelay longer every time.
							iceCreamProgress = 0f;
						}
					}
				}

				if (Input.GetKeyUp(KeyCode.Space))
				{
					iceCreamProgress = 0f;
				}
			}
		}

		UIController.Instance.UpdateUI();
	}
}
