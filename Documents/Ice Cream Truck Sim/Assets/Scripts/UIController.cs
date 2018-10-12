using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
	public Image scorePanel;
	public Text scoreDisplay;
	public Text timeDisplay;
	public Image helpPanel;
	public Text helpDisplay;
	public Slider iceCreamSaleProgress;

	public Image gameOverPanel;
	public Text gameOverScore;

	string iceCreamHelp = "Stop and hold SPACE to sell Ice Cream!";
	string scoreText = "Score: ";

	static UIController instance;
	public static UIController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("UIController");
				instance = go.GetComponent<UIController>();
			}

			return instance;
		}
	}

	void Start()
	{
		iceCreamSaleProgress.maxValue = GameController.Instance.iceCreamDelay;

		//Force the Slider's max value to be greater than 0.
		if (iceCreamSaleProgress.maxValue <= 0f)
			iceCreamSaleProgress.maxValue = 1f;

		iceCreamSaleProgress.enabled = false;

		gameOverPanel.gameObject.SetActive(false);
		for(int i = 0; i < gameOverPanel.transform.childCount; i++)
		{
			gameOverPanel.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void ToggleHelpPanel(bool isOn)
	{
		helpPanel.enabled = isOn;
		
		if (isOn)
			helpDisplay.text = iceCreamHelp;
		else
			helpDisplay.text = "";
	}

	public void MainMenuPressed()
	{
		SceneManager.LoadScene("mainMenu");
	}

	public void ExitPressed()
	{
		//ToDo: Ask player if sure.
		Application.Quit();
	}

	public void UpdateUI()
	{
		switch (GameController.Instance.State)
		{
			case GameState.Playing:
			//ToDo: Make sure pause menu is hidden
			UpdateHUD();
			break;

			case GameState.Paused:
			//ToDo: Display pause menu
			break;

			case GameState.GameOver:
			gameOverPanel.gameObject.SetActive(true);
			for(int i = 0; i < gameOverPanel.transform.childCount; i++)
			{
				gameOverPanel.transform.GetChild(i).gameObject.SetActive(true);
			}
			gameOverScore.text = "Final Score: " + GameController.Instance.score; //ToDo: For secret ending, set to ???
			break;

			default:
			break;
		}
	}

	void UpdateHUD()
	{
		scoreDisplay.text = scoreText + GameController.Instance.score;

		float minutes = 0f;
		float seconds = GameController.Instance.timeLeft;
		
		if (seconds > 60f)
		{
			do
			{
				seconds -= 60f;
				minutes++;
			} while (seconds > 60f);
		}

		//ToDo: Let the FollowerManager know, because if the follower hits the player while they are selling ice cream the game should end (?)

		//ToDo: if seconds == 0.# make sure the 0 is displayed
		timeDisplay.text = minutes + ":" + seconds.ToString("#.00");

		if (Input.GetKeyDown(KeyCode.Space))
			iceCreamSaleProgress.enabled = true;

		if (Input.GetKeyUp(KeyCode.Space))
			iceCreamSaleProgress.enabled = false;

		iceCreamSaleProgress.value = GameController.Instance.iceCreamProgress;
	}
}
