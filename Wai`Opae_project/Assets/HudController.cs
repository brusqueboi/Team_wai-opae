using Assets;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour 
{
	public Vector3 fishDeathTextOffset;
    public Color roiDeathTextColor;
	public Color preyDeathTextColor;

	public Slider healthBar;
    public float fishDeathTextDuration;

	public Text countdownTimer;
	private TextStrobeController countdownTimerStrobe;
	private Color countdownTimerColor;

	public Text countdownTimerOutline;
	private TextStrobeController countdownTimerOutlineStrobe;
	private Color countdownTimerOutlineColor;

	public Text fishCaughtRoi;
	public Text fishCaughtOutlineRoi;
	private float fishCaughtRoiTime;

	public Text fishCaughtP1;
	public Text fishCaughtOutlineP1;
	private float fishCaughtTimeP1;

	public Text fishCaughtP2;
	public Text fishCaughtOutlineP2;
	private float fishCaughtTimeP2;

	public Text fishCaughtP3;
	public Text fishCaughtOutlineP3;
	private float fishCaughtTimeP3;

	public Text fishCaughtP4;
	public Text fishCaughtOutlineP4;
	private float fishCaughtTimeP4;

	public Image shootRoiIndicatorImage;
	public Image savePreyIndicatorImage;
	public float shootRoiIndicatorDuration = 2.5f;
	private float lastPreyShotTime = 0.0f;

    public void Start()
    {
		countdownTimerStrobe = countdownTimer.GetComponent<TextStrobeController>();
		countdownTimerStrobe.strobeEnabled = false;
		countdownTimer.text = string.Empty;
		countdownTimerColor = countdownTimer.color;

		countdownTimerOutlineStrobe = countdownTimerOutline.gameObject.GetComponent<TextStrobeController>();
		countdownTimerOutlineStrobe.strobeEnabled = false;
		countdownTimerOutline.text = string.Empty;
		countdownTimerOutlineColor = countdownTimerOutline.color;

		GameModel.Model.PreyConsumed += (sender, args) =>
		{
			ShowFishDeathText(args.PreyObject, null);
        };
		GameModel.Model.FishCaught += (sender, args) =>
		{
			ShowFishDeathText(args.Target, args.Player);
        };
		
    }

    public void Update()
    {
		// Update shoot roi indicator.
		float shootRoiIndicatorAlpha = (lastPreyShotTime == 0.0f ? 0.0f : Mathf.Clamp(
			((shootRoiIndicatorDuration - (Time.time - lastPreyShotTime)) / (shootRoiIndicatorDuration)) * 2.0f, 0.0f, 1.0f));
		shootRoiIndicatorImage.color = new Color(
			shootRoiIndicatorImage.color.r, shootRoiIndicatorImage.color.g, shootRoiIndicatorImage.color.b, shootRoiIndicatorAlpha);
		savePreyIndicatorImage.color = new Color(
			savePreyIndicatorImage.color.r, savePreyIndicatorImage.color.g, savePreyIndicatorImage.color.b, shootRoiIndicatorAlpha);
		// Update remaining time.
		if (GameModel.Model.RemainingTime > 0.0f && !GameModel.Model.GameSuspended)
		{
			countdownTimer.text = ((int)GameModel.Model.RemainingTime + 1.0f).ToString();
			countdownTimerOutline.text = countdownTimer.text;
		}
		else
		{
			countdownTimer.text = string.Empty;
			countdownTimerOutline.text = string.Empty;
		}
		// Update health bar.
		if (healthBar.maxValue != GameModel.Model.PreyPopulationSize + GameModel.Model.RoiPopulationSize)
			healthBar.maxValue = GameModel.Model.PreyPopulationSize + GameModel.Model.RoiPopulationSize;

		if (healthBar.value != GameModel.Model.RoiPopulationSize)
			healthBar.value = GameModel.Model.RoiPopulationSize;

		countdownTimerStrobe.strobeEnabled = 
			!GameModel.Model.GameSuspended && !GameModel.Model.AnimationSuspended && GameModel.Model.RemainingTime < 11.0f;
		countdownTimerOutlineStrobe.strobeEnabled = countdownTimerStrobe.strobeEnabled;
		
		// Update fish death text.
		fishCaughtRoi.gameObject.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f);
		fishCaughtOutlineRoi.gameObject.transform.position = fishCaughtRoi.gameObject.transform.position;
		float fishCaughtRoiAlpha = Mathf.Clamp(
			(fishDeathTextDuration - (Time.time - fishCaughtRoiTime)) / (fishDeathTextDuration), 0.0f, 1.0f);
		fishCaughtRoi.color = new Color(
			fishCaughtRoi.color.r, fishCaughtRoi.color.g, fishCaughtRoi.color.b, fishCaughtRoiAlpha);
		fishCaughtOutlineRoi.color = new Color(
			fishCaughtOutlineRoi.color.r, fishCaughtOutlineRoi.color.g, fishCaughtOutlineRoi.color.b, fishCaughtRoiAlpha);

		// Player 1
		fishCaughtP1.gameObject.transform.position = Camera.main.WorldToScreenPoint(
			GameModel.Model.GetPlayer(1).Cursor.gameObject.transform.position + fishDeathTextOffset);
		fishCaughtOutlineP1.gameObject.transform.position = fishCaughtP1.gameObject.transform.position;
		float fishCaughtAlphaP1 = (fishCaughtTimeP1 == 0.0f ? 0.0f : Mathf.Clamp(
			(fishDeathTextDuration - (Time.time - fishCaughtTimeP1)) / (fishDeathTextDuration), 0.0f, 1.0f));
		fishCaughtP1.color = new Color(
			fishCaughtP1.color.r, fishCaughtP1.color.g, fishCaughtP1.color.b, fishCaughtAlphaP1);
		fishCaughtOutlineP1.color = new Color(
			fishCaughtOutlineP1.color.r, fishCaughtOutlineP1.color.g, fishCaughtOutlineP1.color.b, fishCaughtAlphaP1);

		// Player 2
		fishCaughtP2.gameObject.transform.position = Camera.main.WorldToScreenPoint(
			GameModel.Model.GetPlayer(2).Cursor.gameObject.transform.position + fishDeathTextOffset);
		fishCaughtOutlineP2.gameObject.transform.position = fishCaughtP2.gameObject.transform.position;
		float fishCaughtAlphaP2 = (fishCaughtTimeP1 == 0.0f ? 0.0f : Mathf.Clamp(
			(fishDeathTextDuration - (Time.time - fishCaughtTimeP2)) / (fishDeathTextDuration), 0.0f, 1.0f));
		fishCaughtP2.color = new Color(
			fishCaughtP2.color.r, fishCaughtP2.color.g, fishCaughtP2.color.b, fishCaughtAlphaP2);
		fishCaughtOutlineP2.color = new Color(
			fishCaughtOutlineP2.color.r, fishCaughtOutlineP2.color.g, fishCaughtOutlineP2.color.b, fishCaughtAlphaP2);

		// Player 3
		fishCaughtP3.gameObject.transform.position = Camera.main.WorldToScreenPoint(
			GameModel.Model.GetPlayer(3).Cursor.gameObject.transform.position + fishDeathTextOffset);
		fishCaughtOutlineP3.gameObject.transform.position = fishCaughtP3.gameObject.transform.position;
		float fishCaughtAlphaP3 = (fishCaughtTimeP1 == 0.0f ? 0.0f : Mathf.Clamp(
			(fishDeathTextDuration - (Time.time - fishCaughtTimeP3)) / (fishDeathTextDuration), 0.0f, 1.0f));
		fishCaughtP3.color = new Color(
			fishCaughtP3.color.r, fishCaughtP3.color.g, fishCaughtP3.color.b, fishCaughtAlphaP3);
		fishCaughtOutlineP3.color = new Color(
			fishCaughtOutlineP3.color.r, fishCaughtOutlineP3.color.g, fishCaughtOutlineP3.color.b, fishCaughtAlphaP3);
		
		// Player 4
		fishCaughtP4.gameObject.transform.position = Camera.main.WorldToScreenPoint(
			GameModel.Model.GetPlayer(4).Cursor.gameObject.transform.position + fishDeathTextOffset);
		fishCaughtOutlineP4.gameObject.transform.position = fishCaughtP4.gameObject.transform.position;
		float fishCaughtAlphaP4 = (fishCaughtTimeP1 == 0.0f ? 0.0f : Mathf.Clamp(
			(fishDeathTextDuration - (Time.time - fishCaughtTimeP4)) / (fishDeathTextDuration), 0.0f, 1.0f));
		fishCaughtP4.color = new Color(
			fishCaughtP4.color.r, fishCaughtP4.color.g, fishCaughtP4.color.b, fishCaughtAlphaP4);
		fishCaughtOutlineP4.color = new Color(
			fishCaughtOutlineP4.color.r, fishCaughtOutlineP4.color.g, fishCaughtOutlineP4.color.b, fishCaughtAlphaP4);
	}

    private void ShowFishDeathText(AbstractFishController fish, PlayerModel player)
    {
		if (player != null && fish is PreyController)
		{
			lastPreyShotTime = Time.time;
		}
		if (player == null)
		{
			fishCaughtRoi.text = "Roi ate " + fish.CommonName;
			fishCaughtOutlineRoi.text = fishCaughtRoi.text;
			fishCaughtRoiTime = Time.time;
			fishCaughtRoi.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
			return;
		}
		switch (player.PlayerIndex)
		{
			case 1:
				fishCaughtP1.text = "-1 " + fish.CommonName;
				fishCaughtOutlineP1.text = fishCaughtP1.text;
				fishCaughtTimeP1 = Time.time;
				fishCaughtP1.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
				break;
			case 2:
				fishCaughtP2.text = "-1 " + fish.CommonName;
				fishCaughtOutlineP2.text = fishCaughtP2.text;
				fishCaughtTimeP2 = Time.time;
				fishCaughtP2.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
				break;
			case 3:
				fishCaughtP3.text = "-1 " + fish.CommonName;
				fishCaughtOutlineP3.text = fishCaughtP3.text;
				fishCaughtTimeP3 = Time.time;
				fishCaughtP3.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
				break;
			case 4:
				fishCaughtP3.text = "-1 " + fish.CommonName;
				fishCaughtOutlineP3.text = fishCaughtP3.text;
				fishCaughtTimeP3 = Time.time;
				fishCaughtP4.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
				break;
			default:
				fishCaughtRoi.text = "-1 " + fish.CommonName;
				fishCaughtOutlineRoi.text = fishCaughtRoi.text;
				fishCaughtRoiTime = Time.time;
				fishCaughtRoi.color = (fish is RoiController ? roiDeathTextColor : preyDeathTextColor);
				break;
		}
    }
	
}
