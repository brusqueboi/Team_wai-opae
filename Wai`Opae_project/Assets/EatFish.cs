/*==============================================================================

==============================================================================*/

using UnityEngine;
using System.Collections;

public class EatFish : MonoBehaviour 
{
    public delegate void EatFishEvent(string fishCommonName, string fishScientificName);
    public static event EatFishEvent OnEatFish;


	#region Public Variables
	
	
	
	#endregion //Public Variables
	
	
	
	#region Private Variables
	
	
	
	#endregion //Private Variables
	

	
	#region Unity Engine & Events
	
	private void OnTriggerEnter(Collider collider)
    {
        //if this object collides with a good fish
        if(collider.CompareTag("Good Fish"))
        {
            //get a reference to the fish name script
            FishName _fishName = collider.GetComponent<FishName>();

            //call event passing the eaten fish names
            if (OnEatFish != null)
                OnEatFish(_fishName.commonName, _fishName.scientificName);

            //destroy the fish
            Destroy(collider.gameObject);
        }
    }
	
	#endregion //Unity Engine & Events
	
	
	
	#region Public Methods
	
	
	
	#endregion //Public Methods
	
	
	
	#region Private Methods
	
	
	
	#endregion //Private Methods
	
}
