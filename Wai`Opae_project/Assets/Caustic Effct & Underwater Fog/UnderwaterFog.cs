/*==============================================================================

==============================================================================*/

using UnityEngine;
using System.Collections;

public class UnderwaterFog : MonoBehaviour 
{
    #region Public Variables

    [Header("Settings")]

    [Tooltip("The Y position of the water level in the scene")]
    public float waterLevel = 3f;

    [Tooltip("This is the density of the underwater forg")]
    public float fogDensity = 0.13f;

    [Tooltip("The color of the underwater fog")]
    public Color fogColor;

    #endregion //Public Variables



    #region Private Variables

    //initial values of the fog, used to reset the fog when not underwater
    private Color initialFogColor;
    private float initialFogDensity;

    private bool isUnderwater;
	
	#endregion //Private Variables
	

	
	#region Unity Engine & Events
	
	private void Awake()
    {
        Cache();
    }

    private void Update()
    {
        //is this camera below water level?
        if(transform.position.y < waterLevel)
        {
            //if it is let's be sure we are not already set underwater
            if (!isUnderwater)
                SetUnderwater();
        }
        else
        {
            if (isUnderwater)
                SetOutsideWater();
        }
    }
	
	#endregion //Unity Engine & Events
	
	
	
	#region Public Methods
	
	
	
	#endregion //Public Methods
	
	
	
	#region Private Methods
	
	private void SetUnderwater()
    {
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        isUnderwater = true;
    }

    private void SetOutsideWater()
    {
        RenderSettings.fogColor = initialFogColor;
        RenderSettings.fogDensity = initialFogDensity;
        isUnderwater = false;
    }

    private void Cache()
    {
        RenderSettings.fog = true;
        initialFogColor = RenderSettings.fogColor;
        initialFogDensity = RenderSettings.fogDensity;
    }

	
	#endregion //Private Methods
	
}
