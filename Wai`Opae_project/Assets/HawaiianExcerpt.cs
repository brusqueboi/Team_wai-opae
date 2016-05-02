using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HawaiianExcerpt : MonoBehaviour {

    #region Public Variables

    public Text HawaiianText;
    
    #endregion //Public Variables

    #region Private Variables

    #endregion //Private Variable

    public void Start()
    {
        HawaiianText.text = "Add Hawaiian stuff here";
    }

    public void Update()
    {
        if(Input.GetButtonDown("AButton_P1"))
        {
            SceneManager.LoadScene("Project");
        }
    }

    #region Private Methods
    
    #endregion //Private Methods
}
