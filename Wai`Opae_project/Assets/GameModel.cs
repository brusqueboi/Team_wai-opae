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

	public event EventHandler GameSuspendedChanged;
	public event EventHandler LevelChanged;
	public event PreyConsumedEventHandler PreyConsumed;

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
	}

	void Update()
	{

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

}
