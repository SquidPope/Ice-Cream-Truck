using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamZone : MonoBehaviour 
{
	//ToDo: Should we "deactivate" this zone after ice cream is sold here once? Deactivate until we're shuffled around?
	bool isActive = false;
	new Renderer renderer;

	void Start()
	{
		renderer = gameObject.GetComponent<Renderer>();
		UIController.Instance.ToggleHelpPanel(isActive);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Truck")
		{
			isActive = true;
			UIController.Instance.ToggleHelpPanel(isActive);
			GameController.Instance.TruckInZone = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Truck")
		{
			isActive = false;
			UIController.Instance.ToggleHelpPanel(isActive);
			GameController.Instance.TruckInZone = false;
		}
	}

	void Update()
	{
		if (isActive)
			renderer.material.color = Color.green;
		else
			renderer.material.color = Color.red;
	}
}
