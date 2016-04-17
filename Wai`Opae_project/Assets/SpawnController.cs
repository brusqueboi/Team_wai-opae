using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

	public GameObject lookAtTarget;

	public GameObject[] spawnLocations;

	private static SpawnController controller;
	public static SpawnController Controller {
		get { return controller; }
	}

	public void Respawn(GameObject respawnObject)
	{
		Spawn(respawnObject);
	}

	public Vector3 SelectSpawnLocation()
	{
		if (spawnLocations != null && spawnLocations.Length > 0)
		{
			return spawnLocations[(int)((Random.value * spawnLocations.Length) + 0.5f)].transform.position;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public void Spawn(GameObject obj)
	{
		Vector3 spawnLocation = SelectSpawnLocation();
		obj.transform.position = spawnLocation;
		obj.transform.LookAt(lookAtTarget.transform);
	}

	// Use this for initialization
	void Start () {
		controller = this;
		spawnLocations = GameObject.FindGameObjectsWithTag("Respawn");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
