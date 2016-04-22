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
    public delegate void CountDownEvent();
    public static event CountDownEvent OnCountDownFinish;
    public AudioClip tcount;
    public AudioClip fishfood;
    public float fvol = 0.7f;
    public float tvol = 0.3f;
    private AudioSource tsource { get { return GetComponent<AudioSource>(); } }

    #region Public Variables

    [Header("Countdown Timer Settings")]

    [Tooltip("The amount of seconds to countdown")]
    public int seconds = 30;

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

    [Tooltip("Fade speed of the eat info panel")]
    public float fadingSpeed = 2f;

    public Image eatInfoPanel;
    public Text eatInfoPanelText;   

    #endregion //Public Variables



    #region Private Variables

    private WaitForSeconds oneSecond;
    private WaitForSeconds waitEatPanelTimer;

    private Color eatInfoPanel_initialColor;

    private float eatInfoPanel_lerpAmount;
	
	#endregion //Private Variables
	

	
	#region Unity Engine & Events
	
	private void Awake()
    {
        Cache();
    }

    private void OnEnable()
    {
        //event subscribe
        EatFish.OnEatFish += OnEatFish;
    }

    private void OnEatFish(string fishCommonName, string fishScientificName)
    {
        playFishEatenSound();

        //we remove one good fish, decreasing the max number from the health bar
        healthBar.maxValue = GameModel.Model.PreyPopulationSize;

        //show the info panel with the common name
        StartCoroutine(ShowEatInfoPanel(fishCommonName));
    }

    private void OnDisable()
    {
        //event unsubscribe
        EatFish.OnEatFish -= OnEatFish;
    }

    private void Start()
    {
        //display initial amount of seconds set on the countdown timer
        countDownTimer.text = seconds.ToString();
        //start countdown
        StartCoroutine(CountDown());
        gameObject.AddComponent<AudioSource>();
        tsource.playOnAwake = false;
    }

    private void Update()
    {
        UpdateHealthBar();
    }
	
	#endregion //Unity Engine & Events
	
	
	
	#region Public Methods
	
	
	
	#endregion //Public Methods
	
	
	
	#region Private Methods
	
	private void Cache()
    {
        oneSecond = new WaitForSeconds(1);
        waitEatPanelTimer = new WaitForSeconds(eatPanelTimer);
        //set the health bar max value = to the amount of fish
        healthBar.maxValue = (GameModel.Model.RoiPopulationSize + GameModel.Model.PreyPopulationSize);

        //save initial color of the panel
        eatInfoPanel_initialColor = eatInfoPanel.color;
    }  

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

    private IEnumerator ShowEatInfoPanel(string fishName)
    {
        //set initial and reset the lerp amount to 0
        eatInfoPanel.color = eatInfoPanel_initialColor;
        eatInfoPanel_lerpAmount = 0f;
        //write name on the UI
        eatInfoPanelText.text = "-1 " + fishName;
        //enable the panel
        eatInfoPanel.gameObject.SetActive(true);
        //wait the set amount of seconds
        yield return waitEatPanelTimer;

        //fade off the panel
        while(eatInfoPanel.color != Color.clear)
        {
            //slowly fade
            eatInfoPanel_lerpAmount += fadingSpeed * Time.deltaTime;
            eatInfoPanel.color = Color.Lerp(eatInfoPanel_initialColor, Color.clear, eatInfoPanel_lerpAmount);
            yield return null;
        }
        //disable the game panel
        eatInfoPanel.gameObject.SetActive(false);
    }

    private IEnumerator CountDown()
    {
        while (seconds > 0)
        {
            if(seconds <= 10)
            {
                playTimerSound();
            }
            //wait for one second
            yield return oneSecond;
            //remove one second from our total seconds
            seconds--;
            //update the amount of seconds displayed
            countDownTimer.text = seconds.ToString();           
        }

        //call countdown finish event
        if (OnCountDownFinish != null)
            OnCountDownFinish();

        //print a message on console at the end of countdown
        if (Debug.isDebugBuild)
            print("Countdown is over!");
    }

    void playTimerSound()
    {
        tsource.PlayOneShot(tcount, tvol);
    }

    void playFishEatenSound()
    {
        tsource.PlayOneShot(fishfood, fvol);
    }
	
	#endregion //Private Methods
	
}
