using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets;
using UnityEngine.UI;

public class GameModel
{
	// Events
	public event EventHandler GameSuspendedChanged;
	public event EventHandler LevelChanged;
	public event EventHandler EndgameDetected;
	public event EventHandler<PreyConsumedEventArgs> PreyConsumed;
	public event EventHandler<FishCaughtEventArgs> FishCaught;

	public static readonly int MinLevel = 0;
	public static readonly int MaxLevel = Int32.MaxValue;

	protected List<OleloNoeau> oleloNoeau = new List<OleloNoeau>();
	protected LinkedList<PreyController> preyPopulation = new LinkedList<PreyController>();
	protected LinkedList<RoiController> roiPopulation = new LinkedList<RoiController>();
	protected LinkedList<PreyController> consumedPrey = new LinkedList<PreyController>();

	protected bool started = false;
	public bool Started { get { return started; } }

	protected float baseLevelDuration = 20.0f;
	public float BaseLevelDuration
	{
		get { return baseLevelDuration; }
		set { baseLevelDuration = value; }
	}

	protected float marginalLevelDuration = 5.0f;
	public float MarginalLevelDuration
	{
		get { return marginalLevelDuration; }
		set { marginalLevelDuration = value; }
	}

	protected float maxLevelDuration = 45.0f;
	public float MaxLevelDuration
	{
		get { return maxLevelDuration; }
		set { maxLevelDuration = value; }
	}

	protected static GameModel model = new GameModel();
	public static GameModel Model { get { return model; }}

	protected PlayerModel[] players = new PlayerModel[4];
	public PlayerModel[] Players { get { return (PlayerModel[]) players.Clone(); }}

	public float ElapsedTime { get { return TotalTime - remainingTime; }}

	protected float remainingTime = 0.0f;
	public float RemainingTime { get { return remainingTime; } }

	protected float totalTime = 0.0f;
	public float TotalTime
	{
		get { return totalTime; }
		set { totalTime = Mathf.Clamp(value, 0.0f, GetMaxTime(Level)); }
	}

	protected int level = 0;
	public int Level
	{
		get { return level; }
		set
		{
			if (value != level)
			{
				if (value < MinLevel || value > MaxLevel)
				{
					Debug.Log("refused out of range level set: " + value);
				}
				else if(level != value)
				{
					level = value;
					ResetFishPopulations();
					GameSuspended = false;
					totalTime = GetMaxTime(level);
					remainingTime = totalTime;
					if(LevelChanged != null)
					{
						LevelChanged.Invoke(this, new EventArgs());
					}
				}
			}
		}
	}

	protected bool gameSuspended = true;
	public bool GameSuspended
	{
		get { return gameSuspended; }
				set
		{
			if (gameSuspended != value)
			{
				gameSuspended = value;
				if(GameSuspendedChanged != null)
				{
					GameSuspendedChanged.Invoke(this, new EventArgs());
				}
			}
		}
	}

	protected bool timeoutTriggersEndgame = true;
	public bool TimeoutTriggersEndgame
	{
		get { return timeoutTriggersEndgame; }
		set { timeoutTriggersEndgame = value; }
	}

	protected bool animationSuspended = false;
	public bool AnimationSuspended
	{
		get { return animationSuspended; }
		set { animationSuspended = value; }
	}

	public int PreyPopulationSize {	get { return preyPopulation.Count; }}

	public PreyController[] PreyPopulation {
		get
		{
			PreyController[] preyPopCopy = new PreyController[preyPopulation.Count];
            preyPopulation.CopyTo(preyPopCopy, 0);
			return preyPopCopy;
		}
	}

	public int RoiPopulationSize { get { return roiPopulation.Count; }}

	public RoiController[] RoiPopulation
	{
		get
		{
			RoiController[] roiPopCopy = new RoiController[roiPopulation.Count];
			roiPopulation.CopyTo(roiPopCopy, 0);
			return roiPopCopy;
		}
	}

	protected GameModel()
	{
		
	}

	public PlayerModel GetPlayer(int playerId)
	{
		if(playerId <= 0 || playerId > 4)
		{
			return null;
		}
		return players[playerId - 1];
	}

	public string GetEnglishOleloNoeau(int level)
	{
		return getOleloNoeau(level).EnglishText;
	}

	public string GetHawaiianOleloNoeau(int level)
	{
		return getOleloNoeau(level).HawaiianText;
	}

	public int SpeciesCount(string targetCommonName)
	{
		if(targetCommonName.Equals("Roi"))
		{
			return RoiPopulationSize;
		}
		else
		{
			int count = 0;
			foreach (PreyController prey in preyPopulation)
			{
				if(prey != null && prey.Alive && prey.CommonName.Equals(targetCommonName))
				{
					count++;
				}
			}
			return count;
		}
	}

	public void Start()
	{
		players[0] = PlayerModel.BuildPlayerModel(1);
		players[1] = PlayerModel.BuildPlayerModel(2);
		players[2] = PlayerModel.BuildPlayerModel(3);
		players[3] = PlayerModel.BuildPlayerModel(4);
		for (int i = 0; i < players.Length; i++)
		{
			int playerId = i;
			if (players[playerId] != null)
			{
				players[playerId].Start();
				players[playerId].Launcher.ProjectileHit += (sender, args) =>
				{
					if (args.Target is AbstractFishController)
					{
						players[playerId].CaughtFish.Add(args.Target);
						if(args.Target is PreyController)
						{
							preyPopulation.Remove(args.Target as PreyController);
							if(PreyPopulationSize < 2 && EndgameDetected != null)
							{
								EndgameDetected(this, new EventArgs());
							}
						}
						else if(args.Target is RoiController)
						{
							roiPopulation.Remove(args.Target as RoiController);
							if(RoiPopulationSize == 0 && EndgameDetected != null)
							{
								EndgameDetected(this, new EventArgs());
							}
						}
					}
					FireFishCaught(players[playerId], args.Target);
				};
			}
			started = true;
		}

		remainingTime = GetMaxTime(level);
		InitOleloNoeaus();
		SpawnController.Controller.FishSpawn += (sender, args) =>
		{
			if (args.SpawnedObject is PreyController)
			{
				preyPopulation.AddLast((PreyController)args.SpawnedObject);
			}
			else if (args.SpawnedObject is RoiController)
			{
				roiPopulation.AddLast((RoiController)args.SpawnedObject);
				((RoiController)args.SpawnedObject).PreyConsumed += (consumedSender, consumedArgs) =>
				{
					consumedPrey.AddLast(consumedArgs.PreyObject);
					preyPopulation.Remove(consumedArgs.PreyObject);
					FirePreyConsumed(consumedArgs.PreyObject, consumedArgs.RoiObject);
					if(PreyPopulationSize < 2 && EndgameDetected != null)
					{
						EndgameDetected(this, new EventArgs());
					}
				};
			}
			else
			{
				Debug.Log("Unrecognized fish spawned: " + args.SpawnedObject.name);
			}
		};

		EndgameDetected += (sender, args) =>
		{
			GameSuspended = true;
		};
	}

	public void Update()
	{
		if (!GameSuspended && remainingTime >= 0.0f)
		{
			remainingTime = Mathf.Clamp((remainingTime - Time.deltaTime), 0.0f, GetMaxTime(Level));
			if (remainingTime == 0.0f && TimeoutTriggersEndgame)
			{
				if (EndgameDetected != null)
				{
					EndgameDetected(this, new EventArgs());
				}
			}
		}
		// Update player models.
		for(int i = 0; i < players.Length; i++)
		{
			if(players[i] != null)
			{
				players[i].Update();
            }
		}
	}

	private void ResetFishPopulations()
	{
		foreach(RoiController roi in roiPopulation)
		{
			if(roi.Alive)
			{
				GameObject.Destroy(roi.gameObject);
			}
		}
		foreach (PreyController prey in preyPopulation)
		{
			if (prey.Alive)
			{
				GameObject.Destroy(prey.gameObject);
			}
		}
		roiPopulation.Clear();
		preyPopulation.Clear();
	}

	private float GetMaxTime(int level)
	{
		return Mathf.Clamp((baseLevelDuration + (marginalLevelDuration * (level - 1))), 
			baseLevelDuration, maxLevelDuration);
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

	protected void FirePreyConsumed(PreyController prey, RoiController roi)
	{
		if (PreyConsumed != null)
		{
			PreyConsumed.Invoke(this, new PreyConsumedEventArgs(prey, roi));
		}
	}

	protected void FireFishCaught(PlayerModel player, AbstractFishController target)
	{
		if (FishCaught != null)
		{
			FishCaught.Invoke(this, new FishCaughtEventArgs(player, target));
		}
	}

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

	public class FishCaughtEventArgs : EventArgs
	{
		public PlayerModel Player { get; private set; }

		public AbstractFishController Target { get; private set; }

		public FishCaughtEventArgs(PlayerModel player, AbstractFishController target)
		{
			Player = Utils.RequireNonNull(player);
			Target = target;
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
		oleloNoeau.Add(new OleloNoeau("Mālama i ke kala ka i‘a hi‘u ‘oi", 
			"Watch out for the kala, the fish with a sharp tail", -1, true));
		oleloNoeau.Add(new OleloNoeau("He manini ka i‘a, mai hō‘ā i ke ahi",
			"The fish is just a manini, so do not light a fire", 1, true));
		oleloNoeau.Add(new OleloNoeau("‘A‘ohe ia e loa‘a aku, he ulua kāpapa no ka moana",
			"He will not be caught, for he is a parrotfish, slippery with slime", 2, true));
		oleloNoeau.Add(new OleloNoeau("Mālama i ke kala ka i‘a hi‘u ‘oi",
			"He cannot be caught for he is an ulua fish of the deep ocean", 3, true));
	}

}
