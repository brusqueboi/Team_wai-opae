using UnityEngine;
using System.Collections;
using System;
using Assets;
using System.Collections.Generic;

public class SpawnController {

	public delegate void FishSpawnEventHandler(object sender, FishSpawnEventArgs args);

	public float roiSpawnRateScale = 0.5f;
	public int initialRoiPopSize = 3;
	public float preySpawnRateScale = 1.0f;
	public int preyInitialPopSize = 8;
	public float preyMarginalLevelSpawnCount = 1.4f;
	public float minLevelSpawnDurationFraction = 0.1f;
	public float maxLevelSpawnDurationFraction = 0.5f;
	public int levelDifficultyCap = 5;

	private LevelSpawnInfo spawnInfo;
	private LevelSpawnInfo remainingSpawnInfo;
	private float curLevelRoiSpawnDelay = 0.0f;
	private float curLevelPreySpawnDelay = 0.0f;
	private float lastRoiSpawnTime = 0.0f;
	private float lastPreySpawnTime = 0.0f;
	private int spawnId = 0;
	private LevelSpawnInfo levelZeroSpawnInfo = new LevelSpawnInfo(10, 30, 0.0f);

	private LinkedList<GameObject> preySpawnQueue = new LinkedList<GameObject>();
	private LinkedList<GameObject> roiSpawnQueue = new LinkedList<GameObject>();

	private GameObject roiReference;
	private GameObject parrotReference;
	private GameObject maniniReference;
	private GameObject butterflyReference;
	private GameObject wrasseReference;
	private GameObject idolReference;
	private GameObject tangReference;

	public class FishSpawnEventArgs : EventArgs
	{
		public AbstractFishController SpawnedObject { get; private set; }

		public FishSpawnEventArgs(GameObject spawnedObject)
		{
			SpawnedObject = Utils.RequireNonNull(spawnedObject.GetComponent<AbstractFishController>());
		}
	}

	public event EventHandler<FishSpawnEventArgs> FishSpawn;

	public GameObject[] spawnLocations;

	private static SpawnController controller = new SpawnController();
	public static SpawnController Controller {
		get { return controller; }
	}

	public void Respawn(GameObject respawnObject)
	{
		InternalSpawn(respawnObject, true);
	}

	public Vector3 SelectSpawnLocation()
	{
		if (spawnLocations != null && spawnLocations.Length > 0)
		{
			return spawnLocations[(int)(UnityEngine.Random.value * spawnLocations.Length)].transform.position;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public GameObject Spawn(GameObject referenceObject, bool active)
	{
		GameObject spawnClone = GameObject.Instantiate(referenceObject);
		spawnClone.name = referenceObject.name + "_" + spawnId;
		spawnId++;
		InternalSpawn(spawnClone, active);
		FireFishSpawned(spawnClone);
		return spawnClone;
	}

	public void RespawnAll()
	{
		PreyController[] preyPop = GameModel.Model.PreyPopulation;
		foreach(PreyController prey in preyPop)
		{
			if(prey.Alive)
			{
				Respawn(prey.gameObject);
			}
		}
		RoiController[] roiPop = GameModel.Model.RoiPopulation;
		foreach (RoiController roi in roiPop)
		{
			if (roi.Alive)
			{
				Respawn(roi.gameObject);
			}
		}
	}

	// Use this for initialization
	public void Start () {
		controller = this;
		spawnLocations = GameObject.FindGameObjectsWithTag("Respawn");

		roiReference = GameObject.Find("Roi Sprite");
		parrotReference = GameObject.Find("Parrot Sprite");
		maniniReference = GameObject.Find("Manini Sprite");
		butterflyReference = GameObject.Find("Butterfly Sprite");
		wrasseReference = GameObject.Find("Wrasse Sprite");
		idolReference = GameObject.Find("Idol Sprite");
		tangReference = GameObject.Find("Tang Sprite");

		roiReference.SetActive(false);
		parrotReference.SetActive(false);
		maniniReference.SetActive(false);
		butterflyReference.SetActive(false);
		wrasseReference.SetActive(false);
		idolReference.SetActive(false);
		tangReference.SetActive(false);

		spawnInfo = levelZeroSpawnInfo;
		for (int i = 0; i < spawnInfo.preySpawnCount; i++)
		{
			Spawn(SelectRandomPreyFish(), true);
        }
		for (int i = 0; i < spawnInfo.roiSpawnCount; i++)
		{
			Spawn(roiReference, true);
		}

		GameModel.Model.EndgameDetected += (sender, args) =>
		{
			Debug.Log("Endgame detected.");
			spawnInfo = LevelSpawnInfo.SPAWN_DISABLED;
			remainingSpawnInfo = LevelSpawnInfo.SPAWN_DISABLED;
		};

		GameModel.Model.LevelChanged += (sender, args) =>
		{
			spawnInfo = GetLevelSpawnInfo(GameModel.Model.Level);
			remainingSpawnInfo = spawnInfo;
			curLevelRoiSpawnDelay = (GameModel.Model.TotalTime * 
				spawnInfo.levelDurationSpawnFraction) / spawnInfo.roiSpawnCount;
			curLevelRoiSpawnDelay = (GameModel.Model.TotalTime *
				spawnInfo.levelDurationSpawnFraction) / spawnInfo.preySpawnCount;
			lastRoiSpawnTime = 0.0f;
			lastPreySpawnTime = 0.0f;
			preySpawnQueue.Clear();
			roiSpawnQueue.Clear();
			for(int i = 0; i < spawnInfo.preySpawnCount; i++)
			{
				GameObject preyRefObj = SelectRandomPreyFish();
				preySpawnQueue.AddLast(Spawn(preyRefObj, false));
			}
			for (int i = 0; i < spawnInfo.roiSpawnCount; i++)
			{
				GameObject roiRefObj = roiReference;
				roiSpawnQueue.AddLast(Spawn(roiRefObj, false));
			}
			if(GameModel.Model.Level == 0)
			{
				foreach(GameObject roiSpawn in roiSpawnQueue)
				{
					roiSpawn.SetActive(true);
				}
				roiSpawnQueue.Clear();
				foreach (GameObject preySpawn in preySpawnQueue)
				{
					preySpawn.SetActive(true);
				}
				preySpawnQueue.Clear();
			}
			Debug.Log(String.Format(
				"Spawn Information (Level {0} [{1} sec.])- Roi={2}, Prey={3}, Spawn Duration={4} sec.", 
				GameModel.Model.Level, GameModel.Model.TotalTime, spawnInfo.roiSpawnCount, spawnInfo.preySpawnCount, 
				(GameModel.Model.TotalTime * spawnInfo.levelDurationSpawnFraction)));
		};
	}
	
	// Update is called once per frame
	public void Update () {
		if(GameModel.Model.GameSuspended || GameModel.Model.AnimationSuspended)
		{
			return;
		}
		float elapsedTime = GameModel.Model.ElapsedTime;
		float totalTime = elapsedTime + GameModel.Model.RemainingTime;
		float levelDurationPercent = elapsedTime / totalTime;
		if (roiSpawnQueue.First != null && remainingSpawnInfo.roiSpawnCount > 0 && 
			(lastRoiSpawnTime == 0.0f || Time.time >= lastRoiSpawnTime + curLevelRoiSpawnDelay))
		{
			remainingSpawnInfo.roiSpawnCount--;
			lastRoiSpawnTime = Time.time;
			GameObject spawnedRoi = roiSpawnQueue.First.Value;
			roiSpawnQueue.RemoveFirst();
			spawnedRoi.SetActive(true);
		}
		if(preySpawnQueue.First != null && remainingSpawnInfo.preySpawnCount > 0 &&
			(lastPreySpawnTime == 0.0f || Time.time >= lastPreySpawnTime + curLevelPreySpawnDelay))
		{
			remainingSpawnInfo.preySpawnCount--;
			lastPreySpawnTime = Time.time;
			GameObject spawnedPrey = preySpawnQueue.First.Value;
			preySpawnQueue.RemoveFirst();
			spawnedPrey.SetActive(true);
		}
	}

	private void InternalSpawn(GameObject spawnObj, bool active)
	{
		spawnObj.SetActive(active);
		Vector3 spawnLocation = SelectSpawnLocation();
		Vector3 lookAtTarget = new Vector3(0.0f, 1.87f, 0.0f);
		spawnObj.transform.position = spawnLocation;
		spawnObj.transform.LookAt(lookAtTarget);
	}

	private GameObject SelectRandomPreyFish()
	{
		int randIdx = (int) (UnityEngine.Random.value * 6.0f + 0.5f);
		switch(randIdx)
		{
			case 0:
				return parrotReference;
			case 1:
				return maniniReference;
			case 2:
				return butterflyReference;
			case 3:
				return wrasseReference;
			case 4:
				return idolReference;
			case 5:
				return tangReference;
			default:
				return parrotReference; // Failsafe.
		}
	}

	private LevelSpawnInfo GetLevelSpawnInfo(int level)
	{
		if(level == 0)
		{
			return levelZeroSpawnInfo;
		}
		LevelSpawnInfo spawnInfo = new LevelSpawnInfo();
		spawnInfo.roiSpawnCount = initialRoiPopSize + (int) ((level * level) * roiSpawnRateScale);
		spawnInfo.preySpawnCount = preyInitialPopSize + (int) ((level - 1.0f) * preyMarginalLevelSpawnCount);
		spawnInfo.levelDurationSpawnFraction = minLevelSpawnDurationFraction + 
			((1.0f - (((float)level - 1.0f) / ((float)levelDifficultyCap - 1.0f))) * 
			(maxLevelSpawnDurationFraction - minLevelSpawnDurationFraction));
        spawnInfo.levelDurationSpawnFraction = Mathf.Clamp(spawnInfo.levelDurationSpawnFraction, 
			minLevelSpawnDurationFraction, maxLevelSpawnDurationFraction);
		return spawnInfo;
	}

	private void FireFishSpawned(GameObject spawnedObject)
	{
		FishSpawn.Invoke(this, new FishSpawnEventArgs(spawnedObject));
	}

	public struct LevelSpawnInfo
	{
		public static readonly LevelSpawnInfo SPAWN_DISABLED = new LevelSpawnInfo(0, 0, 0.0f);

		public int roiSpawnCount;
		public int preySpawnCount;
		public float levelDurationSpawnFraction;

		public LevelSpawnInfo(int roiSpawnCount, int preySpawnCount, float levelDurationSpawnFraction)
		{
			this.roiSpawnCount = roiSpawnCount;
			this.preySpawnCount = preySpawnCount;
			this.levelDurationSpawnFraction = levelDurationSpawnFraction;
		}
	}
}
