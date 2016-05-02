using UnityEngine;
using Assets;
using System.Collections.Generic;

public class LevelProgVeiwController : MonoBehaviour {

	public const float ONE_STAR = 0.10f;
	public const float TWO_STARS = 0.30f;
	public const float THREE_STARS = 0.50f;

	private bool enabled = true; // Enable/disable "press any button to continue..." mechanic.
	private GameObject gameObjRef;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		gameObjRef = gameObject;
		GameModel.Model.EndgameDetected += (sender, args) =>
		{
			UpdateViewForLevel(GameModel.Model.Level, CalculateRating(), 
				GameModel.Model.GetEnglishOleloNoeau(GameModel.Model.Level), 
				GameModel.Model.GetHawaiianOleloNoeau(GameModel.Model.Level));
			gameObjRef.SetActive(true);
		};
	}
	
	// Update is called once per frame
	void Update () {
		if(enabled && (AnyButtonPressed(1) || AnyButtonPressed(2) || AnyButtonPressed(3) || AnyButtonPressed(4)))
		{
			GameModel.Model.Level++;
			gameObject.SetActive(false);
		}
	}

	public void UpdateViewForLevel(int level, int stars, string oleloNoeauEnglish, string oleloNoeauHawaiian)
	{
		// Perform all UI updates to prepare view for specified level.
	}

	protected int CalculateRating()
	{
		PreyController[] preyPop = GameModel.Model.PreyPopulation;
		List<string> uniqueSpecies = new List<string>();
		foreach(PreyController prey in preyPop)
		{
			if(!uniqueSpecies.Contains(prey.CommonName))
			{
				uniqueSpecies.Add(prey.CommonName);
			}
		}

		float biodiversity = (float)(uniqueSpecies.Count + 1/*Roi species*/) / 
			(float)(GameModel.Model.RoiPopulationSize + GameModel.Model.PreyPopulationSize);

		int rating = 0;
		if (biodiversity >= THREE_STARS)
			rating = 3;
		else if (biodiversity >= TWO_STARS)
			rating = 2;
		else if (biodiversity >= ONE_STAR)
			rating = 1;
		return rating;
	}

	protected bool AnyButtonPressed(int playerId)
	{
		ControllerModel playerController = GameModel.Model.GetPlayer(playerId).Controller;
		return GameModel.Model.GetPlayer(playerId).Enabled && (playerController.AButton || playerController.BButton 
			|| playerController.XButton || playerController.YButton || playerController.LeftBumper 
			|| playerController.RightBumper);
	}
}
