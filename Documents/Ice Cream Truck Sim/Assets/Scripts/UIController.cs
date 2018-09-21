using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
	public Image scorePanel;
	public Text scoreDisplay;
	public Text timeDisplay;
	public Image helpPanel;
	public Text helpDisplay;
	string iceCreamHelp = "Press SPACE to sell Ice Cream!";
	
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

	public void ToggleHelpPanel(bool isOn)
	{
		helpPanel.enabled = isOn;
		
		if (isOn)
			helpDisplay.text = iceCreamHelp;
		else
			helpDisplay.text = "";
	}

	public void UpdateUI()
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
			}while (seconds > 60f);
		}

		timeDisplay.text = minutes + ":" + seconds.ToString("#.00");
	}
}
