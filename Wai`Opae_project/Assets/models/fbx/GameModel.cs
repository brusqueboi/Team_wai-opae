using UnityEngine;
using System.Collections;
using System;

public class GameModel : MonoBehaviour
{

	public event EventHandler GameSuspendedChanged;
	public event EventHandler LevelChanged;

	public const int MinLevel = 1;
	public const int MaxLevel = Int32.MaxValue;

	private int level = 1;
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

	private static readonly GameModel MODEL_SINGLETON = new GameModel();

	public static GameModel getInstance()
	{
		return MODEL_SINGLETON;
	}



	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

}
