using UnityEngine;
using System.Collections;
using Assets;
using System.Collections.Generic;

public class PlayerModel {

	public const string UnformattedPlayerIdSuffix = "_P{0}";
	protected static readonly PlayerInitInfo BasePlayerInitInfo =
		new PlayerInitInfo(-1, "PlayerCursor", "PlayerLauncher");

	public int PlayerIndex { get; private set; }
	protected bool enabled = true;
	public bool Enabled {
		get	{ return enabled; }
		set
		{
			if(value != enabled)
			{
				enabled = value;
				Cursor.Visible = value;
				Launcher.Visible = value;
			}
		}
	}
	public ControllerModel Controller { get; protected set; }
	public CursorController Cursor { get; protected set; }
	public LauncherController Launcher { get; protected set; }

	protected List<AbstractFishController> caughtFish = new List<AbstractFishController>();
	public List<AbstractFishController> CaughtFish { get { return caughtFish; } }

	public PlayerModel(int playerIndex, ControllerModel controller, CursorController cursor, LauncherController launcher)
	{
		PlayerIndex = playerIndex;
		Controller = controller;
		Cursor = cursor;
		Launcher = launcher;
	}

	public static PlayerModel BuildPlayerModel(int playerIdx)
	{
		if (playerIdx < 1 || playerIdx > 4)
		{
			Debug.Log("Failed to create player: " + playerIdx);
			return null;
		}
		ControllerModel controller = null;
        switch (playerIdx)
		{
			case 1:
				controller = new ControllerModel(ControllerModel.Player1ButtonUIDs);
				break;
			case 2:
				controller = new ControllerModel(ControllerModel.Player2ButtonUIDs);
				break;
			case 3:
				controller = new ControllerModel(ControllerModel.Player3ButtonUIDs);
				break;
			case 4:
				controller = new ControllerModel(ControllerModel.Player4ButtonUIDs);
				break;
		}
		PlayerInitInfo initInfo = GetPlayerInitInfo(playerIdx);
		GameObject cursorObject = GameObject.Find(initInfo.cursorName);
		GameObject launcherObject = GameObject.Find(initInfo.launcherName);
		if (cursorObject == null || launcherObject == null)
		{
			Debug.Log("Failed to create player: " + playerIdx);
			return null;
		}
		CursorController cursor = cursorObject.GetComponent<CursorController>();
		LauncherController launcher = launcherObject.GetComponent<LauncherController>();
		if (cursor == null || launcher == null)
		{
			Debug.Log("Failed to create player: " + playerIdx);
			return null;
		}
		return new PlayerModel(playerIdx, controller, cursor, launcher);
	}

	public override string ToString()
	{
		return "Player " + PlayerIndex;
	}

	// Use this for initialization
	public void Start()
	{
		
	}

	// Update is called once per frame
	public void Update()
	{
		if(Controller.StartButton)
		{
			Enabled = !Enabled;
		}
	}

	protected static PlayerInitInfo GetPlayerInitInfo(int playerIdx)
	{
		PlayerInitInfo baseInitInfoCopy = 
			BasePlayerInitInfo.AppendSuffix(string.Format(UnformattedPlayerIdSuffix, playerIdx));
		baseInitInfoCopy.playerIndex = playerIdx;
		return baseInitInfoCopy;

	}

	public struct PlayerInitInfo
	{
		public int playerIndex;
		public string cursorName;
		public string launcherName;

		public PlayerInitInfo(int playerIndex, string cursorName, string launcherName)
		{
			this.playerIndex = playerIndex;
			this.cursorName = cursorName;
			this.launcherName = launcherName;
		}

		public PlayerInitInfo AppendSuffix(string suffix)
		{
			PlayerInitInfo initInfoCopy = this;
			initInfoCopy.cursorName += suffix;
			initInfoCopy.launcherName += suffix;
			return initInfoCopy;
		}
	}
}
