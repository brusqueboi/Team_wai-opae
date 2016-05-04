using UnityEngine;
using Assets;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelProgViewController : MonoBehaviour {

	public bool Visible
	{
		get
		{
			return gameObject.activeSelf;
		}
		set
		{
			if(Visible != value)
			{
				gameObject.SetActive(value);
				dismissDelayStart = (value ? Time.time : 0.0f);
			}
		}
	}

	public float dismissableDelay = 5.0f;

	public Text mainText;
	public Text levelProgInfoText;
	public Text levelProgInfoTextOutline;
	public Text hawaiianOleloNoeauText;
	public Text englishOleloNoeauText;
	public Text roiRemainingText;
	public Text parrotRemainingText;
	public Text maniniRemainingText;
	public Text butterflyRemainingText;
	public Text wrasseRemainingText;
	public Text idolRemainingText;
	public Text tangRemainingText;
	public Text mainTextOutline;
	public Text hawaiianOleloNoeauTextOutline;
	public Text englishOleloNoeauTextOutline;
	public Text roiRemainingTextOutline;
	public Text parrotRemainingTextOutline;
	public Text maniniRemainingTextOutline;
	public Text butterflyRemainingTextOutline;
	public Text wrasseRemainingTextOutline;
	public Text idolRemainingTextOutline;
	public Text tangRemainingTextOutline;
	public Text dismissText;
	public Text dismissTextOutline;
	public GameObject startView;

	public Color redColor;
	public Color greenColor;

	private GameObject gameObjRef;
	private float dismissDelayStart = 0.0f;
	private TextStrobeController dismissTextStrobe;
	private TextStrobeController dismissTextOutlineStrobe;

	private TextStrobeController roiValueStrobe;
	private TextStrobeController parrotValueStrobe;
	private TextStrobeController maniniValueStrobe;
	private TextStrobeController butterflyValueStrobe;
	private TextStrobeController wrasseValueStrobe;
	private TextStrobeController idolValueStrobe;
	private TextStrobeController tangValueStrobe;
	private TextStrobeController roiValueOutlineStrobe;
	private TextStrobeController parrotValueOutlineStrobe;
	private TextStrobeController maniniValueOutlineStrobe;
	private TextStrobeController butterflyValueOutlineStrobe;
	private TextStrobeController wrasseValueOutlineStrobe;
	private TextStrobeController idolValueOutlineStrobe;
	private TextStrobeController tangValueOutlineStrobe;
	private bool started = false;
	private float RemainingDismissTime
	{
		get { return Mathf.Clamp(dismissableDelay - (Time.time - dismissDelayStart), 0.0f, dismissableDelay); }
	}

	// Use this for initialization
	void Start () {
		gameObjRef = gameObject;
		dismissTextStrobe = dismissText.gameObject.GetComponent<TextStrobeController>();
		dismissTextOutlineStrobe = dismissTextOutline.gameObject.GetComponent<TextStrobeController>();

		roiValueStrobe = roiRemainingText.gameObject.GetComponent<TextStrobeController>();
		roiValueOutlineStrobe = roiRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		parrotValueStrobe = parrotRemainingText.gameObject.GetComponent<TextStrobeController>();
		parrotValueOutlineStrobe = parrotRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		maniniValueStrobe = maniniRemainingText.gameObject.GetComponent<TextStrobeController>();
		maniniValueOutlineStrobe = maniniRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		butterflyValueStrobe = butterflyRemainingText.gameObject.GetComponent<TextStrobeController>();
		butterflyValueOutlineStrobe = butterflyRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		wrasseValueStrobe = wrasseRemainingText.gameObject.GetComponent<TextStrobeController>();
		wrasseValueOutlineStrobe = wrasseRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		idolValueStrobe = idolRemainingText.gameObject.GetComponent<TextStrobeController>();
		idolValueOutlineStrobe = idolRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		tangValueStrobe = tangRemainingText.gameObject.GetComponent<TextStrobeController>();
		tangValueOutlineStrobe = tangRemainingTextOutline.gameObject.GetComponent<TextStrobeController>();

		started = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Visible)
		{
			levelProgInfoText.color = (GameModel.Model.RoiPopulationSize == 0 
				&& GameModel.Model.PreyPopulationSize > 1 ? greenColor : redColor);
			if(RemainingDismissTime > 0.0f)
			{
				dismissText.text = ((int)RemainingDismissTime + 1.0f).ToString();
				dismissTextOutline.text = dismissText.text;
			}
			else
			{
				dismissText.text = "Press any button to continue";
				dismissTextOutline.text = dismissText.text;
				if (Utils.AnyButtonPressed(1, true) || Utils.AnyButtonPressed(2, true) 
					|| Utils.AnyButtonPressed(3, true) || Utils.AnyButtonPressed(4, true))
				{
					gameObject.SetActive(false);
					if (GameModel.Model.RoiPopulationSize > 0 || GameModel.Model.PreyPopulationSize < 2)
					{
						GameModel.Model.Level = 0;
						GameModel.Model.GameSuspended = true;
						startView.SetActive(true);
					}
					else
					{
						GameModel.Model.Level++;
					}
				}
			}
		}
	}

	public void UpdateView(string oleloNoeauEnglish, string oleloNoeauHawaiian)
	{
		hawaiianOleloNoeauText.text = oleloNoeauHawaiian;
		hawaiianOleloNoeauTextOutline.text = hawaiianOleloNoeauText.text;

		englishOleloNoeauText.text = oleloNoeauEnglish;
		englishOleloNoeauTextOutline.text = englishOleloNoeauText.text;

		roiRemainingText.text = GameModel.Model.SpeciesCount("Roi").ToString();
		roiRemainingTextOutline.text = roiRemainingText.text;

		parrotRemainingText.text = GameModel.Model.SpeciesCount("Parrot Fish").ToString();
		parrotRemainingTextOutline.text = parrotRemainingText.text;

		maniniRemainingText.text = GameModel.Model.SpeciesCount("Manini").ToString();
		maniniRemainingTextOutline.text = maniniRemainingText.text;

		butterflyRemainingText.text = GameModel.Model.SpeciesCount("Racoon Butterfly Fish").ToString();
		butterflyRemainingTextOutline.text = butterflyRemainingText.text;

		wrasseRemainingText.text = GameModel.Model.SpeciesCount("Saddle Wrasse").ToString();
		wrasseRemainingTextOutline.text = wrasseRemainingText.text;

		idolRemainingText.text = GameModel.Model.SpeciesCount("Moorish Idol").ToString();
		idolRemainingTextOutline.text = idolRemainingText.text;

		tangRemainingText.text = GameModel.Model.SpeciesCount("Yellow Tang").ToString();
		tangRemainingTextOutline.text = tangRemainingText.text;

		if (GameModel.Model.RoiPopulationSize > 0 || GameModel.Model.PreyPopulationSize < 2)
		{
			mainText.text = "Game Over";
			mainTextOutline.text = mainText.text;
			if(GameModel.Model.RoiPopulationSize > 0)
			{
				levelProgInfoText.text = GameModel.Model.RoiPopulationSize + " Roi remaining";
				StrobeText("Roi");
			}
			else if(GameModel.Model.PreyPopulationSize == 1)
			{
				levelProgInfoText.text = "Too few prey to repopulate";
				StrobeText(GameModel.Model.PreyPopulation[0].commonName);
			}
			else
			{
				levelProgInfoText.text = "All prey eliminated";
				StrobeText("Parrot Fish", "Manini", "Racoon Butterfly Fish", "Saddle Wrasse", "Moorish Idol", "Yellow Tang");
			}
			levelProgInfoText.color = redColor;
			levelProgInfoTextOutline.text = levelProgInfoText.text;
		}
		else
		{
			mainText.text = "Level " + GameModel.Model.Level + " Complete";
			mainTextOutline.text = mainText.text;
			levelProgInfoText.color = greenColor;
			levelProgInfoText.text = "Saved " + GameModel.Model.PreyPopulationSize + " prey fish";
			levelProgInfoTextOutline.text = levelProgInfoText.text;

			List<string> remainingSpecies = new List<string>();
			PreyController[] preyPop = GameModel.Model.PreyPopulation;
			foreach (PreyController remainingPrey in preyPop)
			{
				if (!remainingSpecies.Contains(remainingPrey.CommonName))
				{
					remainingSpecies.Add(remainingPrey.CommonName);
				}
			}
			StrobeText(remainingSpecies.ToArray());
		}
	}

	private void StrobeText(params string[] fishCommonNames)
	{
		if(!GameModel.Model.Started || !started)
		{
			return;
		}
		roiValueStrobe.strobeEnabled = false;
		roiValueOutlineStrobe.strobeEnabled = false;

		parrotValueStrobe.strobeEnabled = false;
		parrotValueOutlineStrobe.strobeEnabled = false;

		maniniValueStrobe.strobeEnabled = false;
		maniniValueOutlineStrobe.strobeEnabled = false;

		butterflyValueStrobe.strobeEnabled = false;
		butterflyValueOutlineStrobe.strobeEnabled = false;

		wrasseValueStrobe.strobeEnabled = false;
		wrasseValueOutlineStrobe.strobeEnabled = false;

		idolValueStrobe.strobeEnabled = false;
		idolValueOutlineStrobe.strobeEnabled = false;

		tangValueStrobe.strobeEnabled = false;
		tangValueOutlineStrobe.strobeEnabled = false;

		if(fishCommonNames != null)
		{
			foreach(string fishCommonName in fishCommonNames)
			{
				switch (fishCommonName)
				{
					case "Roi":
						roiValueStrobe.strobeEnabled = true;
						roiValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Parrot Fish":
						parrotValueStrobe.strobeEnabled = true;
						parrotValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Manini":
						maniniValueStrobe.strobeEnabled = true;
						maniniValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Racoon Butterfly Fish":
						butterflyValueStrobe.strobeEnabled = true;
						butterflyValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Saddle Wrasse":
						wrasseValueStrobe.strobeEnabled = true;
						wrasseValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Moorish Idol":
						idolValueStrobe.strobeEnabled = true;
						idolValueOutlineStrobe.strobeEnabled = true;
						break;
					case "Yellow Tang":
						tangValueStrobe.strobeEnabled = true;
						tangValueOutlineStrobe.strobeEnabled = true;
						break;
				}
			}
		}
	}
}
