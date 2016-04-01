using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameModel : MonoBehaviour
{

	public event EventHandler GameSuspendedChanged;
	public event EventHandler LevelChanged;

	public const int MinLevel = 1;
	public const int MaxLevel = Int32.MaxValue;

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

	protected List<string> oleloNoeau = new List<string>();
	public string[] OleloNoeau
	{
		get
		{
			return oleloNoeau.ToArray();
		}
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
