using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {Playing, Paused, GameOver, Secret}
public class GameController : MonoBehaviour
{
	GameState state = GameState.Playing;
	int score = 0;
	public float timeLimit; //In seconds
	public float timeLeft;
	public float iceCreamDelay; //Also in seconds
	public float iceCreamProgress = 0f;
	bool truckInZone = false;
	bool hasSoldIceCream = false;

	float secretDelay = 30f;
	float secretTimer = 0f;
	bool isTimingSecret = false;

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

			if (state != GameState.Secret)
				UIController.Instance.UpdateUI();
		}
	}

	public int Score
	{
		get { return score; }
		set
		{
			if (value == 2)
			{
				isTimingSecret = true;
			}
			else if (value == 3) //The timing should work as long as they start the sale withing 10 seconds of the delay.
			{
				if (secretTimer >= secretDelay && secretTimer <= secretDelay + iceCreamDelay + 10f)
				{
					//ToDo:Build super awesome amazing secret ending
					Debug.Log("woo");
				}
				else
				{
					isTimingSecret = false;
				}
			}
			else
			{
				isTimingSecret = false;
			}

			HUDController.Instance.SecretDisplay(isTimingSecret);

			score = value;
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
		
		timeLeft = timeLimit;

		string levelName = SceneManager.GetActiveScene().name;

		if (levelName == "elsewhere")
		{
			State = GameState.Secret;
			HUDController.Instance.UpdateHUD();
		}
		else
		{
			State = GameState.Playing;
			UIController.Instance.UpdateUI();
		}
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
			if (isTimingSecret)
			{
				secretTimer += Time.deltaTime;
			}

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
								Score++;
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
		else if (State == GameState.Paused)
		{
			UIController.Instance.UpdateUI();
		}
		else if (State == GameState.Secret)
		{
			timeLeft -= Time.deltaTime;
			HUDController.Instance.UpdateHUD();
		}
		

		
	}
}
