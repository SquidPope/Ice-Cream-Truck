using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour 
{
	public Image mainPanel;
	public Text highScore;
	public Image optionsPanel;
	string highScoreText = "Player High Score: ";

	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		highScore.text = highScoreText + XMLController.Instance.GetHighScore();
		ToggleOptionsPanel(false);
	}

	public void OnStartPressed()
	{
		if (XMLController.Instance.GetSecretStatus())
			SceneManager.LoadScene("elsewhere");
		else
			SceneManager.LoadScene("neighborhood");
	}

	public void OnExitPressed()
	{
		Application.Quit();
	}

	public void OnOptionsPressed()
	{
		ToggleMainPanel(false);

		//ToDo: Rotate over time
		Camera.main.transform.Rotate(0f, 90f, 0f);
		
		ToggleOptionsPanel(true);
		
	}

	public void OnOptionsBackPressed()
	{
		ToggleOptionsPanel(false);

		Camera.main.transform.Rotate(0f, -90f, 0f);

		ToggleMainPanel(true);
	}

	void ToggleMainPanel(bool isOn)
	{
		mainPanel.gameObject.SetActive(isOn);
		for (int i = 0; i < mainPanel.transform.childCount; i++)
		{
			mainPanel.transform.GetChild(i).gameObject.SetActive(isOn);
		}
	}
	void ToggleOptionsPanel(bool isOn)
	{
		optionsPanel.gameObject.SetActive(isOn);
		for (int i = 0; i < optionsPanel.transform.childCount; i++)
		{
			optionsPanel.transform.GetChild(i).gameObject.SetActive(isOn);
		}
	}
}
