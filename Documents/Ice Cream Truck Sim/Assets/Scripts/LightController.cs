using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour 
{
	new Light light;

	void Start() 
	{
		light = gameObject.GetComponent<Light>();
		light.intensity = XMLController.Instance.Options.screenBrightness;
	}

	void Update()
	{
		if (XMLController.Instance.Options.screenBrightness != light.intensity)
			light.intensity = XMLController.Instance.Options.screenBrightness;
	}
}
