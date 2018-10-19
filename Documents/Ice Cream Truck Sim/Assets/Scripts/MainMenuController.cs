using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour 
{
	public Text highScore;
	string highScoreText = "Player High Score: ";

	void Start()
	{
		highScore.text = highScoreText + XMLController.Instance.GetHighScore();
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
}
