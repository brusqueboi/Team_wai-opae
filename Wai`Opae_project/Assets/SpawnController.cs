using UnityEngine;
using System.Collections;
using System;
using Assets;

public class SpawnController {

	public delegate void FishSpawnEventHandler(object sender, FishSpawnEventArgs args);

	public class FishSpawnEventArgs : EventArgs
	{
		public AbstractFishController SpawnedObject { get; private set; }

		public FishSpawnEventArgs(GameObject spawnedObject)
		{
			SpawnedObject = Utils.RequireNonNull(spawnedObject.GetComponent<AbstractFishController>());
		}
	}

	public event FishSpawnEventHandler OnFishSpawn;

	public GameObject lookAtTarget;

	public GameObject[] spawnLocations;

	private static SpawnController controller = new SpawnController();
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
			return spawnLocations[(int)((UnityEngine.Random.value * spawnLocations.Length) + 0.5f)].transform.position;
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
		FireFishSpawned(obj);
	}

	// Use this for initialization
	public void Start () {
		controller = this;
		spawnLocations = GameObject.FindGameObjectsWithTag("Respawn");
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

	private void FireFishSpawned(GameObject spawnedObject)
	{
		OnFishSpawn.Invoke(this, new FishSpawnEventArgs(spawnedObject));
	}
}
