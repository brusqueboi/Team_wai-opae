using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;

public class NeighborCollectionController : MonoBehaviour {

	public int neighborCount = 0;

	public LinkedList<AbstractFishController> Neighbors { get { return neighbors; } }
	protected LinkedList<AbstractFishController> neighbors = new LinkedList<AbstractFishController>();

	void OnTriggerEnter(Collider collisionInfo)
	{
		AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if (controller != null && !neighbors.Contains(controller))
		{
			neighbors.AddLast(controller);
			neighborCount = neighbors.Count;
		}
	}

	void OnTriggerExit(Collider collisionInfo)
	{
		AbstractFishController controller = Utils.GetAbstractFishController(collisionInfo.gameObject);
		if (controller != null)
		{
			neighbors.Remove(controller);
			neighborCount = neighbors.Count;
		}
	}
}
