using UnityEngine;
using System.Collections;

public class DaylightController : MonoBehaviour {

	public float horizontalSpread = 20.0f;
	public float horizontalSpeed = 0.01f;

	private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float newHorizontalPos = (originalPosition.x - (horizontalSpread / 2.0f)) 
			+ Mathf.PingPong(Time.time * horizontalSpeed, horizontalSpread);
		transform.position = new Vector3(newHorizontalPos, transform.position.y, transform.position.z);
	}
}
