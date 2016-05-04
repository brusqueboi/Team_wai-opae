using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextStrobeController : MonoBehaviour {

	public float strobeRate = 1.0f; // 1 hz.
	public float minAlpha = 0.3f;
	public float maxAlpha = 1.0f;

	public bool strobeEnabled = true;

	private Color textColor;
	private Text textComponent;

	// Use this for initialization
	void Start () {
		textComponent = GetComponent<Text>();
		if(textComponent != null)
		{
			textColor = textComponent.color;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(textComponent != null && strobeEnabled)
		{
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 
				Mathf.Clamp(minAlpha + Mathf.PingPong(Time.time, 1.0f / (strobeRate * 2.0f)) * (strobeRate * 2.0f), minAlpha, maxAlpha));
		}
		else
		{
			textComponent.color = textColor;
		}
	}
}
