using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour 
{
	public float amplitude;
	float originY;

	void Start()
	{
		originY = transform.position.y;
	}

	void Update()
	{
		float y = originY + (amplitude * Mathf.Sin(Time.time));
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}
}
