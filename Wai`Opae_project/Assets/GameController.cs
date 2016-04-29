using UnityEngine;
using System.Collections;
using System;
using Assets;

public class GameController : MonoBehaviour {

	public delegate void FishSpawnedEventHandler(object sender, FishSpawnedEventArgs args);

	public class FishSpawnedEventArgs : EventArgs {
		public AbstractFishController SpawnedObject { get; private set; }
		public FishSpawnedEventArgs(AbstractFishController spawnedObject)
		{
			SpawnedObject = Utils.RequireNonNull(spawnedObject);
		}
	}

	public event FishSpawnedEventHandler FishSpawned;

	private static GameController controller = null;
	public static GameController Controller
	{
		get { return controller; }
	}

    private AudioController audioController = new AudioController();

	// Use this for initialization
	void Start () {
		controller = this;
		GameModel.Model.Start();
		SpawnController.Controller.Start();
        audioController.Start();
	}
	
	// Update is called once per frame
	void Update () {
		// Quit the application by pressing ESC key.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		GameModel.Model.Update();
		SpawnController.Controller.Update();
        audioController.Update();
	}

	protected void FireFishSpawnedEvent(GameObject spawnedObject)
	{
		FishSpawned.Invoke(this, new FishSpawnedEventArgs(Utils.GetAbstractFishController(spawnedObject)));
	}
}
