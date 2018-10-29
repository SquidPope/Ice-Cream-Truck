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
	public Image creditsPanel;
	string highScoreText = "Player High Score: ";

	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		highScore.text = highScoreText + XMLController.Instance.GetHighScore();
		TogglePanel(optionsPanel, false);
		TogglePanel(creditsPanel, false);
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
		TogglePanel(mainPanel, false);
		TogglePanel(optionsPanel, true);
		
	}

	public void OnOptionsBackPressed()
	{
		TogglePanel(optionsPanel, false);
		TogglePanel(mainPanel, true);
	}

	public void OnCreditsPressed()
	{
		TogglePanel(mainPanel, false);
		TogglePanel(creditsPanel, true);
	}

	public void OnCreditsBackPressed()
	{
		TogglePanel(creditsPanel, false);
		TogglePanel(mainPanel, true);
	}

	void TogglePanel(Image panel, bool isOn)
	{
		panel.gameObject.SetActive(isOn);
		for (int i = 0; i < panel.transform.childCount; i++)
		{
			panel.transform.GetChild(i).gameObject.SetActive(isOn);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			Application.Quit();
	}
}
