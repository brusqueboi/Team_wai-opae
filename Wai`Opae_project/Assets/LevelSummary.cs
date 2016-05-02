using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSummary : MonoBehaviour {

    #region Public Variables

    public Text summary;

    public Text levelText;

    public Text winText;

    public Text roiCaught;

    public Text fishSaved;

    public Text parrotFish;

    public Text manini;

    public Text moorishIdol;

    public Text racoonButterflyFish;

    public Text saddleWrasse;

    public Text yellowTang;

    public Text pressAnyButton;

    public int playerId;

    #endregion //Public Variables

    #region Private Variables

    private bool nothing = true;

    #endregion //Private Variable

    public void Start()
    {
        setLevelText();
        setFishText();
    }

    public void Update()
    {
        checkButton();
        StartCoroutine(flashAnyButton());
    }

    #region Private Methods

    void setLevelText()
    {
        summary.text = "Summary";
        levelText.text = "Level: " + GameModel.Model.Level.ToString();
        winText.text = "You Win!";
        pressAnyButton.text = "Press Any Button to Continue";
    }

    void setFishText()
    {
        parrotFish.text = "Parrot Fish: ";
        manini.text = "Manini: ";
        racoonButterflyFish.text = "Raccoon Butterfly Fish: ";
        saddleWrasse.text = "Saddle Wrasse: ";
        moorishIdol.text = "Moorish Idol: ";
        yellowTang.text = "Yellow Tang: ";

        roiCaught.text = "Roi Caught: ";
        fishSaved.text = "Fish saved: ";
    }


    private IEnumerator flashAnyButton()
    {
        while(nothing == true)
        {
            pressAnyButton.text = "";
            checkButton();
            yield return new WaitForSeconds(1.5f);
            pressAnyButton.text = "Press Any Button to Continue";
            checkButton();
            yield return new WaitForSeconds(1.5f);
            checkButton();
        }
    }

    void checkButton()
    {
        if(Input.GetButtonDown("AButton_P1"))
        {
            nothing = false;
            SceneManager.LoadScene("HawaiianExcerpt");
        }
    }
    
    #endregion //Private Methods

}
