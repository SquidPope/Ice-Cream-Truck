using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public Image scorePanel;
	public Text scoreDisplay;
	public Text timeDisplay;
	string scoreText = "Score: ";

	static HUDController instance = null;
	public static HUDController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("UIController");
				instance = go.GetComponent<HUDController>();
			}

			return instance;
		}
	}

	public void SecretDisplay(bool isOn)
	{
		if (isOn)
			timeDisplay.color = new Color(116f, 0f, 0f, 1f);
		else
			timeDisplay.color = Color.black;
	}

	public void UpdateHUD()
	{
		scoreDisplay.text = scoreText + GameController.Instance.Score;
		timeDisplay.text = "";

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

		if (minutes < 0 || seconds < 0)
		{
			timeDisplay.text = "-";
		}

		//ToDo: Let the FollowerManager know, because if the follower hits the player while they are selling ice cream the game should end (?)

		//ToDo: if seconds == 0.# make sure the 0 is displayed
		timeDisplay.text += minutes + ":" + seconds.ToString("#.00");
	}
}
