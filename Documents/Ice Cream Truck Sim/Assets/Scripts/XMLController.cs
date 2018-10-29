using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsData
{
	public int mapSize; //ToDo: is there a reason to make this a Vector2?
	public float volume;
	public float mouseSensitivity;
	public Resolution screenResolution;
	public bool fullscreen;

	public OptionsData()
	{
		mapSize = 5;
		volume = 1f;
		mouseSensitivity = 1f;

		screenResolution = Screen.resolutions[Screen.resolutions.Length - 1];
		fullscreen = true;
	}
}
public class SaveData
{
	public int highScore;
	public bool isSecret;
	public OptionsData options;

	public SaveData()
	{
		highScore = 0;
		isSecret = false;

		options = new OptionsData();
	}
}

public class XMLController : MonoBehaviour 
{
	string path;
	SaveData save;
	XmlSerializer serializer;

	static XMLController instance;
	public static XMLController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("XMLController");

				if (go != null)
				{
					instance = go.GetComponent<XMLController>();
				}
				else
				{
					//If an XMLController doesn't already exist in the scene, make one.
					GameObject obj = new GameObject("XMLController");
					instance = obj.AddComponent<XMLController>();
					obj.tag = "XMLController";
				}	
			}

			return instance;
		}
	}

	public OptionsData Options
	{
		get { return save.options; }
		set { save.options = value; }
	}

	public int GetHighScore()
	{
		return save.highScore;
	}

	public bool GetSecretStatus()
	{
		return save.isSecret;
	}

	public int GetMapSize()
	{
		return save.options.mapSize;
	}

	void Awake()
	{
		path = Application.persistentDataPath + "\\QurvFocTot.acxc";
		serializer = new XmlSerializer(typeof (SaveData));
		DontDestroyOnLoad(gameObject);

		Load();

		if (save.isSecret && SceneManager.GetActiveScene().name == "mainMenu")
			SceneManager.LoadScene("mainMenuSecret");
	}

	public void Save()
	{
		if (save == null)
			save = new SaveData();

		if (GameObject.FindGameObjectWithTag("GameController"))
		{
			if (GameController.Instance.Score > save.highScore)
				save.highScore = GameController.Instance.Score;

			save.isSecret = GameController.Instance.State == GameState.Secret;
		}

		using (FileStream stream = new FileStream(path, FileMode.Create))
		{
			XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
			serializer.Serialize(writer, save);
		}
	}

	public void Load()
	{
		if (!File.Exists(path))
			Save();

		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			save = serializer.Deserialize(stream) as SaveData;
		}
	}
}
