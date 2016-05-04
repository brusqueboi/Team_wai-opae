using UnityEngine;
using Assets;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartViewController : MonoBehaviour
{
	public bool Visible
	{
		get
		{
			return gameObject.activeSelf;
		}
		set
		{
			if (Visible != value)
			{
				gameObject.SetActive(value);
				dismissDelayStart = (value ? Time.time : 0.0f);
			}
		}
	}

	public float dismissableDelay = 5.0f;

	public Text hawaiianOleloNoeau;
	public Text hawaiianOleloNoeauOutline;
	public Text englishOleloNoeau;
	public Text englishOleloNoeauOutline;
	public Text dismissText;
	public Text dismissTextOutline;

	private GameObject gameObjRef;
	private float dismissDelayStart = 0.0f;
	private TextStrobeController dismissTextStrobe;
	private TextStrobeController dismissTextOutlineStrobe;
	private float RemainingDismissTime
	{
		get { return Mathf.Clamp(dismissableDelay - (Time.time - dismissDelayStart), 0.0f, dismissableDelay); }
	}

	// Use this for initialization
	void Start()
	{
		gameObject.SetActive(true);
		gameObjRef = gameObject;
		dismissTextStrobe = dismissText.gameObject.GetComponent<TextStrobeController>();
		dismissTextOutlineStrobe = dismissTextOutline.gameObject.GetComponent<TextStrobeController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Visible)
		{
			if (RemainingDismissTime > 0.0f)
			{
				dismissText.text = ((int)RemainingDismissTime + 1.0f).ToString();
				dismissTextOutline.text = dismissText.text;
			}
			else
			{
				dismissText.text = "Press any button to play";
				dismissTextOutline.text = dismissText.text;
				if (Utils.AnyButtonPressed(1) || Utils.AnyButtonPressed(2) || Utils.AnyButtonPressed(3) || Utils.AnyButtonPressed(4))
				{
					GameModel.Model.Level++;
					gameObject.SetActive(false);
				}
			}
		}
	}

	public void UpdateView(string oleloNoeauEnglish, string oleloNoeauHawaiian)
	{
		hawaiianOleloNoeau.text = oleloNoeauHawaiian;
		englishOleloNoeau.text = oleloNoeauEnglish;
		hawaiianOleloNoeauOutline.text = oleloNoeauHawaiian;
		englishOleloNoeauOutline.text = oleloNoeauEnglish;
	}
}
