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
    public AudioClip timerTick;
	public GameObject controlsDiagramLeft;
	public GameObject controlsDiagramRight;
	public GameObject controlsDiagramLeftWide;
	public GameObject controlsDiagramRightWide;


	private AudioController audioController = new AudioController();
	public GameObject levelProgView;

	public GameObject uiObject;

	// Use this for initialization
	void Start () {
		controller = this;
        audioController.timerTick = timerTick;
		GameModel.Model.Start();
		SpawnController.Controller.Start();
        audioController.Start();
		GameObject normalCollisionMesh = GameObject.Find("game_environment/1x2_collision_mesh");
		GameObject wideCollisionMesh = GameObject.Find("game_environment/1x3_collision_mesh");

		GameModel.Model.GetPlayer(1).Cursor.gameObject.SetActive(false);
		GameModel.Model.GetPlayer(2).Cursor.gameObject.SetActive(false);
		GameModel.Model.GetPlayer(3).Cursor.gameObject.SetActive(false);
		GameModel.Model.GetPlayer(4).Cursor.gameObject.SetActive(false);

		Vector3 defaultCursorPosP1 = GameModel.Model.GetPlayer(1).Cursor.transform.position;
		Vector3 defaultCursorPosP2 = GameModel.Model.GetPlayer(2).Cursor.transform.position;
		Vector3 defaultCursorPosP3 = GameModel.Model.GetPlayer(3).Cursor.transform.position;
		Vector3 defaultCursorPosP4 = GameModel.Model.GetPlayer(4).Cursor.transform.position;

		bool player1Enabled = true;
		bool player2Enabled = true;
		bool player3Enabled = true;
		bool player4Enabled = true;

		GameModel.Model.GameSuspendedChanged += (sender, args) =>
		{
			if(GameModel.Model.GameSuspended)
			{
				player1Enabled = GameModel.Model.GetPlayer(1).Enabled;
				player2Enabled = GameModel.Model.GetPlayer(2).Enabled;
				player3Enabled = GameModel.Model.GetPlayer(3).Enabled;
				player4Enabled = GameModel.Model.GetPlayer(4).Enabled;

				GameModel.Model.GetPlayer(1).Enabled = true;
				GameModel.Model.GetPlayer(2).Enabled = true;
				GameModel.Model.GetPlayer(3).Enabled = true;
				GameModel.Model.GetPlayer(4).Enabled = true;
			}
			else
			{
				GameModel.Model.GetPlayer(1).Enabled = player1Enabled;
				GameModel.Model.GetPlayer(2).Enabled = player2Enabled;
				GameModel.Model.GetPlayer(3).Enabled = player3Enabled;
				GameModel.Model.GetPlayer(4).Enabled = player4Enabled;
			}
		};

		GameModel.Model.LevelChanged += (sender, args) =>
		{
			GameModel.Model.GetPlayer(1).Cursor.ResetNeighbors();
			GameModel.Model.GetPlayer(2).Cursor.ResetNeighbors();
			GameModel.Model.GetPlayer(3).Cursor.ResetNeighbors();
			GameModel.Model.GetPlayer(4).Cursor.ResetNeighbors();

			GameModel.Model.GetPlayer(1).Cursor.transform.position = defaultCursorPosP1;
			GameModel.Model.GetPlayer(2).Cursor.transform.position = defaultCursorPosP2;
			GameModel.Model.GetPlayer(3).Cursor.transform.position = defaultCursorPosP3;
			GameModel.Model.GetPlayer(4).Cursor.transform.position = defaultCursorPosP4;

			GameModel.Model.GetPlayer(1).Cursor.gameObject.SetActive(GameModel.Model.Level != 0);
			GameModel.Model.GetPlayer(2).Cursor.gameObject.SetActive(GameModel.Model.Level != 0);
			GameModel.Model.GetPlayer(3).Cursor.gameObject.SetActive(GameModel.Model.Level != 0);
			GameModel.Model.GetPlayer(4).Cursor.gameObject.SetActive(GameModel.Model.Level != 0);
		};

		if (wide)
		{
			normalCollisionMesh.SetActive(false);
			wideCollisionMesh.SetActive(true);
			controlsDiagramLeft.SetActive(false);
			controlsDiagramRight.SetActive(false);
			controlsDiagramLeftWide.SetActive(true);
			controlsDiagramRightWide.SetActive(true);
		}
		else
		{
			normalCollisionMesh.SetActive(true);
			wideCollisionMesh.SetActive(false);
			controlsDiagramLeft.SetActive(true);
			controlsDiagramRight.SetActive(true);
			controlsDiagramLeftWide.SetActive(false);
			controlsDiagramRightWide.SetActive(false);
		}

		GameModel.Model.EndgameDetected += (sender, args) =>
		{
			uiObject.SetActive(false);
			levelProgView.GetComponent<LevelProgViewController>().UpdateView(GameModel.Model.GetEnglishOleloNoeau(GameModel.Model.Level),
				GameModel.Model.GetHawaiianOleloNoeau(GameModel.Model.Level));
			levelProgView.GetComponent<LevelProgViewController>().Visible = true;
		};

		GameModel.Model.LevelChanged += (sender, args) =>
		{
			levelProgView.gameObject.SetActive(false);
			uiObject.SetActive(GameModel.Model.Level != 0);
		};
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
		if(GameModel.Model.Started)
		{
			CheckDebugButtonsForPlayer(1);
			CheckDebugButtonsForPlayer(2);
			CheckDebugButtonsForPlayer(3);
			CheckDebugButtonsForPlayer(4);
		}
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
