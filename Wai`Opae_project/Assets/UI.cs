/*==============================================================================
Update 19/04/2016:

-removed tags for good fish and bad fish.
-To get the number of fish the script is now using _gameModel.PreyPopulationSize and _gamemodel.RoiPopulationSize.
 
==============================================================================*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class handles the User Interface
/// </summary>
public class UI : MonoBehaviour 
{
	public Vector3 fishDeathTextOffset;

    public Color alertColor;
    private Color defautColor;

    #region Public Variables

    [Header("Countdown Timer Settings")]

    [Tooltip("Reference to the countdown timer display")]
    public Text countDownTimer;

    [Header("Health Bar Settings")]

    [Tooltip("Referece to the Health Bar")]
    public Slider healthBar;

    [Tooltip("Referece to the Roi amount display")]
    public Text roiAmountDisplay;

    [Tooltip("Referece to the Prey Aount display")]
    public Text preyAmountDisplay;

    [Header("Eat Panel Pop Up Settings")]

    [Tooltip("The amount of time in seconds the panel will remain on the screen before disappearing")]
    public float eatPanelTimer;

    public Text fishDeathText;

	#endregion //Public Variables



	#region Private Variables

	private bool timerAudioPlayed = false;
	private Camera mainCamera;
	private GameObject fishDeathTextTarget = null;
	private float fishDeathTextStartTime = 0.0f;
	
	#endregion //Private Variable

    public void Start()
    {
        //display initial amount of seconds set on the countdown timer
        countDownTimer.text = string.Empty;
        defautColor = countDownTimer.color;
		GameModel.Model.PreyConsumed += (sender, args) =>
		{
			ShowEatInfoPanel(args.PreyObject, null);
        };
		GameModel.Model.FishCaught += (sender, args) =>
		{
			ShowEatInfoPanel(args.Target, args.Player.Cursor.gameObject);
        };
		GameModel.Model.LevelChanged += (sender, args) =>
		{
			timerAudioPlayed = false;
		};
    }

    public void Update()
    {
		if(healthBar.maxValue != GameModel.Model.PreyPopulationSize + GameModel.Model.RoiPopulationSize)
		{
			healthBar.maxValue = GameModel.Model.PreyPopulationSize + GameModel.Model.RoiPopulationSize;
        }
		if(healthBar.value != GameModel.Model.RoiPopulationSize)
		{
			healthBar.value = GameModel.Model.RoiPopulationSize;
		}
        bool useAlertColor = !GameModel.Model.GameSuspended && !GameModel.Model.AnimationSuspended && GameModel.Model.RemainingTime < 11.0f;
        countDownTimer.color = (useAlertColor ? alertColor : defautColor);
        if (fishDeathTextTarget != null)
		{
			if (Time.time < fishDeathTextStartTime + eatPanelTimer)
			{
				fishDeathText.gameObject.transform.position =
					Camera.main.WorldToScreenPoint(fishDeathTextTarget.transform.position + fishDeathTextOffset);
                fishDeathText.color = new Color(
					fishDeathText.color.r, fishDeathText.color.g, fishDeathText.color.b,
					((eatPanelTimer - (Time.time - fishDeathTextStartTime)) / (eatPanelTimer)));
			}
			else
			{
				fishDeathTextTarget = null;
				fishDeathText.text = string.Empty;
				fishDeathText.color = new Color(
					fishDeathText.color.r, fishDeathText.color.g, fishDeathText.color.b, 0.0f);
			}
		}
		else if (Time.time < fishDeathTextStartTime + eatPanelTimer)
		{
			fishDeathText.gameObject.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f);
			fishDeathText.color = new Color(
				fishDeathText.color.r, fishDeathText.color.g, fishDeathText.color.b,
				((eatPanelTimer - (Time.time - fishDeathTextStartTime)) / (eatPanelTimer)));
		}
		if (GameModel.Model.RemainingTime > 0.0f && !GameModel.Model.GameSuspended)
		{
			countDownTimer.text = ((int)GameModel.Model.RemainingTime).ToString();
		}
		else
		{
			countDownTimer.text = string.Empty;
		}
	}
	
	
	
	#region Private Methods

    private void UpdateHealthBar()
    {
        //update health bar
        if (healthBar.value != GameModel.Model.RoiPopulationSize)
            healthBar.value = GameModel.Model.RoiPopulationSize;

        //update Roi counter
        if (GameModel.Model.RoiPopulationSize.ToString() != roiAmountDisplay.text)
            roiAmountDisplay.text = GameModel.Model.RoiPopulationSize.ToString();
        //update prey counter
        if (preyAmountDisplay.text != GameModel.Model.PreyPopulationSize.ToString())
            preyAmountDisplay.text = GameModel.Model.PreyPopulationSize.ToString();
    }

    private void ShowEatInfoPanel(AbstractFishController fish, GameObject target)
    {
		// Enable the panel and show "-1 fish-name".
		fishDeathText.text = "-1 " + fish.CommonName;
		fishDeathTextStartTime = Time.time;
		fishDeathTextTarget = target;
    }
	
	#endregion //Private Methods
	
}
