/*==============================================================================
Update 19/04/2016:

-Remove tags for the Good Fish OnTriggerEnter, the script now check if the collided trigger has a PreyController script attached.
-The Script is no longer using FishName Script to get the name of the collided fish.
To get the name is now using _preyController.commonName and _preyController.scientificName.
==============================================================================*/

using UnityEngine;
using Assets;

public class EatFish : MonoBehaviour 
{
    public delegate void EatFishEvent(string fishCommonName, string fishScientificName);
    public static event EatFishEvent OnEatFish;
	
	private void OnTriggerEnter(Collider collider)
    {
        //if this object collides with an object that has the prey controller script attached
        if(collider.GetComponent<PreyController>())
        {
            //get a reference to the fish name script
            PreyController _preyController = collider.GetComponent<PreyController>();

            //call event passing the eaten fish names
            if (OnEatFish != null)
                OnEatFish(_preyController.commonName, _preyController.scientificName);

            //destroy the collided fish
            Destroy(collider.gameObject);
        }
    }
}
