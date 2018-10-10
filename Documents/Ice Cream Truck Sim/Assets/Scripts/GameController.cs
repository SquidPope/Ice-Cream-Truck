using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Playing, Paused, GameOver, Secret}
public class GameController : MonoBehaviour
{
	GameState state = GameState.Playing;
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

	public GameState State
	{
		get { return state; }

		set
		{
			state = value;
			UIController.Instance.UpdateUI();
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
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (state == GameState.Paused)
			{
				//Play
			}
			else if (state == GameState.Playing)
			{
				//Pause
			}
			else if (state == GameState.GameOver)
			{
				//Load main menu?
			}
		}

		if (State == GameState.Playing)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0)
			{
				state = GameState.GameOver;
				//ToDo: Slow/Stop Truck
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
		}
		

		UIController.Instance.UpdateUI();
	}
}
