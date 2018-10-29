using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
	public Image helpPanel;
	public Text helpDisplay;
	public Slider iceCreamSaleProgress;

	public Image gameOverPanel;
	public Text gameOverScore;
	public Text gameOverHintsText;

	string iceCreamHelp = "Stop and hold SPACE to sell Ice Cream!";
	
	static string[] gameOverHints = new string[] {"ncuq", "xeakfone irakfo gn", "tdian c of", "td nptxmv", "riop ree ulisd", "tdkse fmv", "smianx", "c fow diem", "aknx vttda kutrw", "sbia c for xn"};
	int hintIndex = 0;

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
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;
		
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

		hintIndex = Random.Range(0, gameOverHints.Length);
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
			HUDController.Instance.UpdateHUD();

			if (Input.GetKeyDown(KeyCode.Space))
			iceCreamSaleProgress.enabled = true;

			if (Input.GetKeyUp(KeyCode.Space))
				iceCreamSaleProgress.enabled = false;

			iceCreamSaleProgress.value = GameController.Instance.iceCreamProgress;
			break;

			case GameState.Paused:
			//ToDo: Display pause menu
			break;

			case GameState.GameOver:
			gameOverHintsText.text = gameOverHints[hintIndex]; //if a reset() function happens, just change index
			
			gameOverPanel.gameObject.SetActive(true);
			for(int i = 0; i < gameOverPanel.transform.childCount; i++)
			{
				gameOverPanel.transform.GetChild(i).gameObject.SetActive(true);
			}
			gameOverScore.text = "Final Score: " + GameController.Instance.Score; //ToDo: For secret ending, set to ???
			break;

			default:
			break;
		}
	}
}
