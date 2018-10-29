using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour 
{
	AudioSource source;

	public List<AudioClip> clips;

	static AudioController instance;
	public static AudioController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioController");
				instance = go.GetComponent<AudioController>();
			}

			return instance;
		}
	}

	public void Init()
	{
		source = gameObject.GetComponent<AudioSource>();
		source.volume = 0.3f;
		source.volume *= XMLController.Instance.Options.volume;
		ToggleSound(false);
	}

	public void ToggleSound(bool isOn)
	{
		if (isOn)
		{
			int rand = Random.Range(0, clips.Count);
			source.clip = clips[rand];
			source.Play();
		}
		else
		{
			source.Pause();
		}
	}
}
