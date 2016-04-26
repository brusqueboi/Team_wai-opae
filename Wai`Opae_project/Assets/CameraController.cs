using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float bobDistance = 1.0f;
	public float bobSpeedScale = 1.0f;
	public float bobIntensity = 1.0f;

	private Vector3 originalPosition;
	private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
		lastPosition = originalPosition;
	}
	
	// Update is called once per frame
	void Update () {
		lastPosition.y = Mathf.Clamp(lastPosition.y 
			+ (((Mathf.PerlinNoise(Time.time * bobSpeedScale, 0) * 2.0f) - 1.0f) * bobIntensity * bobDistance * Time.deltaTime), 
			(originalPosition.y - (bobDistance / 2.0f)), (originalPosition.y + (bobDistance / 2.0f)));
		transform.position = lastPosition;
	}
}
