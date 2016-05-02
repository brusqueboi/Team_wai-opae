using UnityEngine;
using Assets;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartViewController : MonoBehaviour
{

	public const float ONE_STAR = 0.10f;
	public const float TWO_STARS = 0.30f;
	public const float THREE_STARS = 0.50f;

	private bool enabled = true; // Enable/disable "press any button to continue..." mechanic.
	private GameObject gameObjRef;

	private Text hawaiianOleloNoeauTxt;
	private Text englishOleloNoeauTxt;

	// Use this for initialization
	void Start()
	{
		gameObject.SetActive(true);
		gameObjRef = gameObject;
		hawaiianOleloNoeauTxt = GameObject.Find("Start View/Olelo Noeau Hawaiian").GetComponent<Text>();
		englishOleloNoeauTxt = GameObject.Find("Start View/Olelo Noeau English").GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		if (enabled && (Utils.AnyButtonPressed(1) || Utils.AnyButtonPressed(2) || Utils.AnyButtonPressed(3)
			|| Utils.AnyButtonPressed(4)))
		{
			GameModel.Model.Level++;
			gameObject.SetActive(false);
		}
	}

	public void UpdateView(string oleloNoeauEnglish, string oleloNoeauHawaiian)
	{
		hawaiianOleloNoeauTxt.text = oleloNoeauHawaiian;
		englishOleloNoeauTxt.text = oleloNoeauEnglish;
	}
}
