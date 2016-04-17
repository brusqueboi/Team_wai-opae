using UnityEngine;
using System.Collections;

public class ObjectRespawnTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter(Collision collisionInfo)
	{
		Debug.Log("Respawning: " + collisionInfo.gameObject.name);
		SpawnController.Controller.Respawn(collisionInfo.gameObject);
	}

}
