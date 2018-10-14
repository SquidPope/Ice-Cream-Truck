using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class Messages : MonoBehaviour 
{
	static string[] messages = new string[] {"Hello?", "Who are you", "I have a gift for you.", "<b>RUN</b>", "why\nwhywhywhywhy", "Do you need your eyes?", "I <b>WANT</b> them", "Where are you?"};
	TextMesh text;

	void Start()
	{
		text = gameObject.GetComponent<TextMesh>();
		RandomizeMessage();
	}

	public void RandomizeMessage()
	{
		int rand = Random.Range(0, messages.Length);
		text.text = messages[rand];
	}
}
