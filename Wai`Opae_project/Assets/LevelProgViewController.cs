using UnityEngine;
using Assets;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelProgViewController : MonoBehaviour {

	public const float ONE_STAR = 0.10f;
	public const float TWO_STARS = 0.30f;
	public const float THREE_STARS = 0.50f;

	private bool enabled = true; // Enable/disable "press any button to continue..." mechanic.
	private GameObject gameObjRef;

	public Text mainText;
	public Text hawaiianOleloNoeauText;
	public Text englishOleloNoeauText;
	public Text roiRemainingText;
	public Text parrotRemainingText;
	public Text maniniRemainingText;
	public Text butterflyRemainingText;
	public Text wrasseRemainingText;
	public Text idolRemainingText;
	public Text tangRemainingText;
	public GameObject startView;

	// Use this for initialization
	void Start () {
		gameObjRef = gameObject;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(enabled && (Utils.AnyButtonPressed(1) || Utils.AnyButtonPressed(2) || Utils.AnyButtonPressed(3) 
			|| Utils.AnyButtonPressed(4)))
		{
			gameObject.SetActive(false);
			if (GameModel.Model.RoiPopulationSize > 1)
			{
				GameModel.Model.Level = 0;
				startView.SetActive(true);
			}
			else
			{
				GameModel.Model.Level++;
			}
		}
	}

	public void UpdateView(string oleloNoeauEnglish, string oleloNoeauHawaiian)
	{
		hawaiianOleloNoeauText.text = oleloNoeauHawaiian;
		englishOleloNoeauText.text = oleloNoeauEnglish;
		roiRemainingText.text = GameModel.Model.SpeciesCount("Roi").ToString();
		parrotRemainingText.text = GameModel.Model.SpeciesCount("Parrot Fish").ToString();
		maniniRemainingText.text = GameModel.Model.SpeciesCount("Manini").ToString();
		butterflyRemainingText.text = GameModel.Model.SpeciesCount("Racoon Butterfly Fish").ToString();
		wrasseRemainingText.text = GameModel.Model.SpeciesCount("Saddle Wrasse").ToString();
		idolRemainingText.text = GameModel.Model.SpeciesCount("Moorish Idol").ToString();
		tangRemainingText.text = GameModel.Model.SpeciesCount("Yellow Tang").ToString();

		if (GameModel.Model.RoiPopulationSize > 1)
		{
			mainText.text = "Game Over";
		}
		else
		{
			mainText.text = "Level " + GameModel.Model.Level + " Complete";
		}
	}
}
