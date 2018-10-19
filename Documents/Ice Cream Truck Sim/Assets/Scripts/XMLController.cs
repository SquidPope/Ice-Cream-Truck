using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData
{
	public int highScore;
	public bool isSecret;
	//ToDo: Options

	public SaveData()
	{
		highScore = 0;
		isSecret = false;
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
				instance = go.GetComponent<XMLController>();
			}

			return instance;
		}
	}

	public int GetHighScore()
	{
		return save.highScore;
	}

	public bool GetSecretStatus()
	{
		return save.isSecret;
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
