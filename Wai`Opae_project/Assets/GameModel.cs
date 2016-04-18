using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets;
using UnityEngine.UI;

public class GameModel : MonoBehaviour
{

	public delegate void PreyConsumedEventHandler(object sender, PreyConsumedEventArgs args);

	public class PreyConsumedEventArgs : EventArgs
	{
		public PreyController PreyObject { get; private set; }

		public RoiController RoiObject { get; private set; }

		public PreyConsumedEventArgs(PreyController prey, RoiController roi)
		{
			PreyObject = Utils.RequireNonNull(prey);
			RoiObject = Utils.RequireNonNull(roi);
		}
	}

	public delegate void RoiCaughtEventHandler(object sender, PreyConsumedEventArgs args);

	public class RoiCaughtEventArgs : EventArgs {
		public PlayerModel Player  { get; private set; }

		public RoiController RoiObject { get; private set; }

		public RoiCaughtEventArgs(PlayerModel player, RoiController roi) {
			Player = Utils.RequireNonNull(player);
			RoiObject = Utils.RequireNonNull(roi);
		}
	}

	public event EventHandler GameSuspendedChanged;
	public event EventHandler LevelChanged;
	public event EventHandler EndgameDetected;
	public event PreyConsumedEventHandler PreyConsumed;
	public event RoiCaughtEventHandler RoiCaught;

	public static readonly int MinLevel = 1;
	public static readonly int MaxLevel = Int32.MaxValue;

	protected static GameModel model = new GameModel();
	public static GameModel Model
	{
		get
		{
			return model;
		}
	}

	protected float elapsedTime = 0.0f;
	public float ElapsedTime { get { return elapsedTime; }}

	protected float remainingTime = 0.0f;
	public float RemainingTime { get { return remainingTime; } }

	protected int level = 1;
	public int Level
	{
		get
		{
			return level;
		}

		set
		{
			if (value != level)
			{
				if (value < MinLevel || value > MaxLevel)
				{
					Debug.Log("refused out of range level set: " + value);
				}
				else
				{
					level = value;
					LevelChanged.Invoke(this, new EventArgs());
				}
			}
		}
	}

	protected bool gameSuspended = true;
	public bool GameSuspended
	{
		get
		{
			return gameSuspended;
		}
		
		set
		{
			if (gameSuspended != value)
			{
				gameSuspended = value;
				GameSuspendedChanged.Invoke(this, new EventArgs());
			}
		}
	}

	public int PreyPopulationSize
	{
		get { return preyPopulation.Count; }
	}

	public int RoiPopulationSize
	{
		get { return roiPopulation.Count; }
	}

	protected List<OleloNoeau> oleloNoeau = new List<OleloNoeau>();
	public string getEnglishOleloNoeau(int level)
	{
		return getOleloNoeau(level).EnglishText;
	}

	public string getHawaiianOleloNoeau(int level)
	{
		return getOleloNoeau(level).HawaiianText;
	}

	private float GetMaxTime(int level)
	{
		return 30.0f;
	}

	private OleloNoeau getOleloNoeau(int level)
	{
		if(level < MinLevel || level > MaxLevel)
		{
			return OleloNoeau.ErrorOleloNoeau;
		}
		// Find olelo whose level designation matches requested.
		OleloNoeau result = oleloNoeau.Find((olelo) => olelo.LevelDesignation == level);
		if(result == null)
		{	// Otherwise find level-agnostic olelo and rotate to back.
			int matchIdx = oleloNoeau.FindIndex((olelo) => olelo.LevelAgnostic);
			if(matchIdx < 0)
			{	// Error recovery.
				result = OleloNoeau.ErrorOleloNoeau;
			}
			else
			{	// Set result and rotate to back of list.
				result = oleloNoeau[matchIdx];
				oleloNoeau.RemoveAt(matchIdx);
				oleloNoeau.Add(result);
			}
		}
		return result;
	}

	private LinkedList<PreyController> preyPopulation = new LinkedList<PreyController>();
	private LinkedList<RoiController> roiPopulation = new LinkedList<RoiController>();

	private GameModel()
	{
		
	}

	void Start()
	{
		GameController.Controller.FishSpawned += (sender, args) =>
		{
			AbstractFishController controller = args.SpawnedObject;
			if(controller is RoiController)
			{
				roiPopulation.AddFirst((RoiController) controller);
			}
			else if(controller is PreyController)
			{
				preyPopulation.AddFirst((PreyController)controller);
			}
			else
			{
				Debug.Log("Unrecognized fish class spawned: " + controller);
			}
		};
		remainingTime = GetMaxTime(level);
		InitOleloNoeaus();
		SpawnController.Controller.OnFishSpawn += (sender, args) =>
		{
			if(args.SpawnedObject is PreyController)
			{
				preyPopulation.AddLast((PreyController) args.SpawnedObject);
			}
			else if (args.SpawnedObject is RoiController)
			{
				roiPopulation.AddLast((RoiController)args.SpawnedObject);
			}
			else
			{
				Debug.Log("Unrecognized fish spawned: " + args.SpawnedObject.name);
			}
		};
	}

	void Update()
	{
		if(remainingTime > 0.0f)
		{
			remainingTime = Mathf.Clamp((remainingTime - Time.deltaTime), 0.0f, GetMaxTime(Level));
			if (remainingTime == 0.0f)
			{
				EndgameDetected.Invoke(this, new EventArgs());
			}
		}
	}

	public class OleloNoeau
	{
		public static readonly OleloNoeau ErrorOleloNoeau = new OleloNoeau("ERROR [Hawaiian]", "ERROR [English]");

		public readonly string HawaiianText;
		public readonly string EnglishText;
		public readonly int LevelDesignation;
		public readonly bool LevelAgnostic;

		public OleloNoeau(string hawaiianText, string englishText) : this(hawaiianText, englishText, -1, true)
		{
			// All setup done by other constructor.
		}

		public OleloNoeau(string hawaiianText, string englishText, int levelDesignation, bool levelAgnostic)
		{
			HawaiianText = hawaiianText;
			EnglishText = englishText;
			LevelDesignation = levelDesignation;
			LevelAgnostic = levelAgnostic;
		}
	}

	private void InitOleloNoeaus()
	{
		oleloNoeau.Add(new OleloNoeau("Mālama i ke kala ka i‘a hi‘u ‘oi.", 
			"Watch out for the kala, the fish with a sharp tail.", -1, true));
		oleloNoeau.Add(new OleloNoeau("He manini ka i‘a, mai hō‘ā i ke ahi.",
			"The fish is just a manini, so do not light a fire.", 1, true));
		oleloNoeau.Add(new OleloNoeau("‘A‘ohe e loa‘a, he uhu pakelo.",
			"Watch out for the kala, the fish with a sharp tail.", 2, true));
		oleloNoeau.Add(new OleloNoeau("Mālama i ke kala ka i‘a hi‘u ‘oi.",
			"He will not be caught, for he is a parrotfish, slippery with slime.", 3, true));
	}

}
