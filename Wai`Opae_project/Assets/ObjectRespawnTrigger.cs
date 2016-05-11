using UnityEngine;
using System.Collections;
using Assets;

public class ObjectRespawnTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider collisionInfo)
	{
		AbstractFishController collisionFish = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if(collisionFish != null && collisionFish.Alive)
		{
			Debug.Log("Respawning: " + collisionInfo.gameObject.name);
			SpawnController.Controller.Respawn(collisionInfo.gameObject);
		}
	}

}
