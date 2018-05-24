using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{
	public Transform targetLookAt;
	public float distance = 3f;
	public float distanceMin = 3f;
	public float distanceMax = 13f;

    //HandlePlayerInput
    float baseMouseSensitivity = 1f;
    float mouseSensitivityMultiplier = 2f;
	public float XmouseSensitivity = 5f;
	public float YmouseSensitivity = 5f;
	public float mouseWheelSensitivity = 5f;
	public float YminLimit = -40f;
	public float YmaxLimit = 80f;
	
	//CalculateDesiredPosition
	public float distanceSmooth = 0.05f;
	float velDistance = 0f;
	Vector3 desiredPosition = Vector3.zero;
	
	//Occlusion test
	float occlusionDistanceStep = 0.5f;
	int maxOcclusionChecks = 10;
	
	//reset desired distance
	float distanceResumeSmooth = 0.5f;
	float currentDistanceSmooth = 0f;
	float preOccludedDist = 0f;
	
	//LateUpdate
	public float Xsmooth = 0.1f;
	public float Ysmooth = 0.2f;
	float velX = 0f;
	float velY = 0f;
	float velZ = 0f;
	Vector3 newPosition = Vector3.zero;
	
	float mouseX = 0f;
	float mouseY = 0f;
	float startDistance = 0f;
	float desiredDistance = 0f;

	public static ThirdPersonCamera instance;

	public float MouseSensitivity 
	{
		get { return mouseSensitivityMultiplier; }
	}

	void Awake()
	{
		instance = this;
		GameObject.DontDestroyOnLoad(transform.gameObject);
	}

	void Start()
	{
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		startDistance = distance;
		Reset();
	}

	void HandlePlayerInput()
	{

	}

	public void Reset()
	{
		mouseX = 0f;
		mouseY = 10f;
		distance = startDistance;
		desiredDistance = distance;
	}

	void CalculateDesiredPosition()
	{

	}

	void UpdatePosition()
	{

	}

	void LateUpdate()
	{
		if (targetLookAt == null)
			return;

		HandlePlayerInput();
		CalculateDesiredPosition();
		UpdatePosition();
	}

	public static void UseExistingOrCreateMainCamera()
	{
		GameObject tempCamera;
		GameObject tempLookAt;
		ThirdPersonCamera cameraScriptInstance;

		if (Camera.main != null)
		{
			tempCamera = Camera.main.gameObject;
		}
		else
		{
			tempCamera = new GameObject("Main Camera");
			tempCamera.AddComponent<Camera>();
			tempCamera.tag = "Main Camera";
		}

		tempCamera.AddComponent<ThirdPersonCamera>();
		cameraScriptInstance = tempCamera.GetComponent<ThirdPersonCamera>();

		tempLookAt = GameObject.Find("targetLookAt");

		if (tempLookAt == null)
		{
			tempLookAt = new GameObject("targetLookAt");
			tempLookAt.transform.position = Vector3.zero;
		}

		cameraScriptInstance.targetLookAt = tempLookAt.transform;
	}
}
