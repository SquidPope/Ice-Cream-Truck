using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextHints : MonoBehaviour 
{
	//When the player turns off their music, chance to display a hint
	Text hintText;
	Color textColor;
	public bool isShowingHint = false;
	static string[] hints = new string[] {"Fail, Pray", "Simple, with the <b>KEY</b>", "It's just a Simulator?!", "Okay, what?", "Pray and Fail?", "Behind you", "Look back", "Fail\n\nPray"};

	float fadeDelay = 0.0075f;

	void Start()
	{
		hintText = gameObject.GetComponent<Text>();
		textColor = hintText.color;
	}

	void ShowHint()
	{
		int rand = Random.Range(0, hints.Length);

		//ToDo: Fade text out over time (fade in too?)
		//ToDo: Randomize font size? or change per hint?
		//ToDo: Don't display if game is over 
		hintText.text = hints[rand];

		isShowingHint = true;

		textColor.a = 1f;
		hintText.color = textColor;
	}

	void Update()
	{
		if (isShowingHint)
		{
			if (hintText.color.a > 0f)
			{
				textColor.a -= fadeDelay;
				hintText.color = textColor;
			}
			else
			{
				isShowingHint = false;
			}
		}
		else
		{
			//check game timer?

			if (Input.GetMouseButtonUp(0) && !TruckController.Instance.IsMusicPlaying)
			{
				ShowHint();
			}
		}
	}
}
