using UnityEngine;
using System.Collections;
using System;
using Assets;

public class GameController : MonoBehaviour {

	private static GameController controller = null;
	public static GameController Controller
	{
		get { return controller; }
	}

	public bool wide = false;

    private AudioController audioController = new AudioController();

	// Use this for initialization
	void Start () {
		controller = this;
		GameModel.Model.Start();
		SpawnController.Controller.Start();
        audioController.Start();
		GameObject normalCollisionMesh = GameObject.Find("game_environment/1x2_collision_mesh");
		GameObject wideCollisionMesh = GameObject.Find("game_environment/1x3_collision_mesh");
		if (wide)
		{
			normalCollisionMesh.SetActive(false);
			wideCollisionMesh.SetActive(true);
		}
		else
		{
			normalCollisionMesh.SetActive(true);
			wideCollisionMesh.SetActive(false);
		}
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
		CheckDebugButtonsForPlayer(1);
		CheckDebugButtonsForPlayer(2);
		CheckDebugButtonsForPlayer(3);
		CheckDebugButtonsForPlayer(4);
	}

	private void CheckDebugButtonsForPlayer(int playerId)
	{
		if (GameModel.Model.GetPlayer(playerId).Controller.BackButton)
		{
			if (GameModel.Model.GetPlayer(playerId).Controller.YButton)
			{
				GameModel.Model.TimeoutTriggersEndgame = !GameModel.Model.TimeoutTriggersEndgame;
				Debug.Log("Timeout Enabled: " + GameModel.Model.TimeoutTriggersEndgame);
			}
			if (GameModel.Model.GetPlayer(playerId).Controller.XButton)
			{
				Debug.Log("Respawning all fish.");
				SpawnController.Controller.RespawnAll();
			}
			if (GameModel.Model.GetPlayer(playerId).Controller.StartButton)
			{
				GameModel.Model.AnimationSuspended = !GameModel.Model.AnimationSuspended;
				GameModel.Model.GameSuspended = GameModel.Model.AnimationSuspended;
			}
		}

		if (GameModel.Model.GetPlayer(playerId).Controller.BButton)
		{
			if (GameModel.Model.GetPlayer(playerId).Controller.DPadUp)
			{
				Debug.Log("Increasing time + 5 sec.");
				GameModel.Model.BaseLevelDuration += 5.0f;
				GameModel.Model.MaxLevelDuration += 5.0f;
				if (GameModel.Model.RemainingTime > 0.0f)
				{
					GameModel.Model.TotalTime += 5.0f;
				}
			}
			if (GameModel.Model.GetPlayer(playerId).Controller.DPadDown)
			{
				Debug.Log("Decreasing time -5 sec.");
				GameModel.Model.BaseLevelDuration -= 5.0f;
				GameModel.Model.MaxLevelDuration -= 5.0f;
				if (GameModel.Model.RemainingTime > 0.0f)
				{
					GameModel.Model.TotalTime -= 5.0f;
				}
			}
			if (GameModel.Model.GetPlayer(playerId).Controller.DPadLeft)
			{
				Debug.Log("Previous Level");
				GameModel.Model.Level--;
			}
			if (GameModel.Model.GetPlayer(playerId).Controller.DPadRight)
			{
				Debug.Log("Next Level");
				GameModel.Model.Level++;
			}

			if (GameModel.Model.GetPlayer(playerId).Controller.StartButton)
			{
				GameModel.Model.GameSuspended = !GameModel.Model.GameSuspended;
				GameModel.Model.AnimationSuspended = GameModel.Model.GameSuspended;
				Debug.Log("Game/Animation Suspended: " + GameModel.Model.AnimationSuspended);
			}
		}
	}
}
