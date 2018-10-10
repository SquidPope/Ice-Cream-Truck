using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{
	public struct ClipPlanePoints
	{
		public Vector3 upperLeft;
		public Vector3 upperRight;
		public Vector3 lowerLeft;
		public Vector3 lowerRight;
	}

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
	public float Xsmooth = 0.05f;
	public float Ysmooth = 0.1f;
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
		//Disable player input while right mouse button is down, for testing purposes.
		if (Input.GetMouseButton(1))
			return;

		mouseX += Input.GetAxis("Mouse X") * XmouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * YmouseSensitivity;

		ClampAngle(mouseY, YminLimit, YmaxLimit);

		float deadZone = 0.01f;

		if (Input.GetAxis("Mouse ScrollWheel") > deadZone || Input.GetAxis("Mouse ScrollWheel") < -deadZone)
		{
			desiredDistance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitivity, distanceMin, distanceMax);
			preOccludedDist = desiredDistance;
			currentDistanceSmooth = distanceSmooth;
		}
	}

	public void Reset()
	{
		mouseX = 0f;
		mouseY = 10f;
		distance = startDistance;
		desiredDistance = distance;
		preOccludedDist = distance;
	}

	void CalculateDesiredPosition()
	{
		ResetDesiredDistance();

		distance = Mathf.SmoothDamp(distance, desiredDistance, ref velDistance, currentDistanceSmooth);
		desiredPosition = CalculatePosition(mouseY, mouseX, distance); //We need to swap x and y to reverse our axies
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0f, 0f, -distance); //Distance is negative because we want to point at our character from behind.
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);

		return targetLookAt.position + rotation * direction; //Return our position relative to the target.
	}

	bool CheckIfOccluded(int count)
	{
		bool isOccluded = false;

		float nearestDist = CheckCameraPoints(targetLookAt.position, desiredPosition);

		if (nearestDist != -1)
		{
			//hit something
			if (count < maxOcclusionChecks)
			{
				isOccluded = true;
				distance -= occlusionDistanceStep;

				if (distance < 0.25f) //Prevent the camera from getting too close.
					distance = 0.25f;
			}
			else
			{
				distance = nearestDist + Camera.main.nearClipPlane;
			}

			desiredDistance = distance;
			distanceSmooth = distanceResumeSmooth;
		}

		return isOccluded;
	}

	public float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		float nearestDist = -1f;

		RaycastHit hitInfo;

		ClipPlanePoints clipPlanePoints = ClipPlaneAtNear(to);

		#region Draw Debug Lines
		//Draw line casts in the editor.
		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red);
		Debug.DrawLine(from, clipPlanePoints.upperLeft, Color.cyan);
		Debug.DrawLine(from, clipPlanePoints.upperRight, Color.cyan);
		Debug.DrawLine(from, clipPlanePoints.lowerLeft, Color.cyan);
		Debug.DrawLine(from, clipPlanePoints.lowerRight, Color.cyan);

		//Visual representation of near clip plane.
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight, Color.green);
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.lowerLeft, Color.green);
		Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight, Color.green);
		Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.lowerRight, Color.green);
		#endregion

		if (Physics.Linecast(from, clipPlanePoints.upperLeft, out hitInfo) && hitInfo.collider.tag != "Truck")
			nearestDist = hitInfo.distance;

		if (Physics.Linecast(from, clipPlanePoints.lowerLeft, out hitInfo) && hitInfo.collider.tag != "Truck")
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;

		if (Physics.Linecast(from, clipPlanePoints.lowerRight, out hitInfo) && hitInfo.collider.tag != "Truck")
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;

		if (Physics.Linecast(from, clipPlanePoints.upperRight, out hitInfo) && hitInfo.collider.tag != "Truck")
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;

		if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Truck")
			if (hitInfo.distance < nearestDist || nearestDist == -1)
				nearestDist = hitInfo.distance;

		return nearestDist;
	}

	void ResetDesiredDistance()
	{
		if (desiredDistance < preOccludedDist) //Is the desired distance closer than pre-occluded distance? If so, the camera has moved because of occlusion of the player.
		{
			//Check if the player would still be occluded if the camera moved back to its pre-occluded distance.
			Vector3 preOccludedPos = CalculatePosition(mouseY, mouseX, preOccludedDist);
			float nearestDist = CheckCameraPoints(targetLookAt.position, preOccludedPos);

			if (nearestDist == -1 || nearestDist > preOccludedDist) //Check if the position at the pre-occluded distance is not occluded at all, or if the 'occlusion' we found is farther away than the position at the pre-occluded distance.
			{
				//Move the camera back towards its pre-occluded distance.
				desiredDistance = preOccludedDist;
			}
		}
	}

	void UpdatePosition()
	{
		float posX = Mathf.SmoothDamp(newPosition.x, desiredPosition.x, ref velX, Xsmooth);
		float posY = Mathf.SmoothDamp(newPosition.y, desiredPosition.y, ref velY, Ysmooth);
		float posZ = Mathf.SmoothDamp(newPosition.z, desiredPosition.z, ref velZ, Xsmooth);
		newPosition = new Vector3(posX, posY, posZ);

		transform.position = newPosition;
		transform.LookAt(targetLookAt);
	}

	void FixedUpdate()
	{
		if (targetLookAt == null)
			return;

		HandlePlayerInput();

		int count = 0;
		do
		{
			CalculateDesiredPosition();
			count++;
		}while(CheckIfOccluded(count));
		
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

	public static float ClampAngle(float angle, float min, float max)
	{
		do
		{
			if (angle > 360)
			{
				angle -= 360;
			}
			else if (angle < -360)
			{
				angle += 360;
			}
		} while (angle > 360 || angle < -360);

		return Mathf.Clamp(angle, min, max);
	}

	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
	{
		ClipPlanePoints cpp = new ClipPlanePoints();

		if (Camera.main == null)
			return cpp;

		Transform cameraTransform = Camera.main.transform;
		float halfFOV = (Camera.main.fieldOfView / 2f) * Mathf.Deg2Rad;
		float aspect = Camera.main.aspect;
		float clipPlaneDist = Camera.main.nearClipPlane;
		float height = clipPlaneDist * Mathf.Tan(halfFOV);
		float width = height * aspect;

		cpp.lowerRight = pos + cameraTransform.right * width;
		cpp.lowerRight -= cameraTransform.up * height;
		cpp.lowerRight += cameraTransform.forward * clipPlaneDist;

		cpp.lowerLeft = pos - cameraTransform.right * width;
		cpp.lowerLeft -= cameraTransform.up * height;
		cpp.lowerLeft += cameraTransform.forward * clipPlaneDist;

		cpp.upperRight = pos + cameraTransform.right * width;
		cpp.upperRight += cameraTransform.up * height;
		cpp.upperRight += cameraTransform.forward * clipPlaneDist;

		cpp.upperLeft = pos - cameraTransform.right * width;
		cpp.upperLeft += cameraTransform.up * height;
		cpp.upperLeft += cameraTransform.forward * clipPlaneDist;

		return cpp;
	}
}
