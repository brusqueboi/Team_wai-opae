using UnityEngine;
using System.Collections;
using Assets;
using System.Collections.Generic;

public class CursorController : MonoBehaviour {

	public int playerId = 1;
	public float yPosition = 1.87f;
	public float maxCursorSpeed = 10.0f;
	public float magnetismRadius = 4.0f;
	public float minMagnetismStrength = 0.5f;
	public float maxMagnetismStrength = 0.5f;

	private NeighborCollectionController neighborCollector;
	private Vector3 defaultCursorPos;

	protected bool visible = true;
	public bool Visible
	{
		get { return visible; }
		set
		{
			if(value != visible)
			{
				visible = value;
				gameObject.GetComponent<Renderer>().enabled = visible;
			}
		}
	}

	public void ResetNeighbors()
	{
		if(neighborCollector != null)
		{
			neighborCollector.Neighbors.Clear();
		}
	}
	
	// Use this for initialization
	void Start ()
    {
		neighborCollector = 
			GameObject.Find(name + "/Magnetism Controller").GetComponent<NeighborCollectionController>();
		neighborCollector.GetComponent<SphereCollider>().radius = magnetismRadius;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameModel.Model.GameSuspended || GameModel.Model.AnimationSuspended || GameModel.Model.Level == 0 
			|| !GameModel.Model.GetPlayer(playerId).Enabled)
		{
			return;
		}
		if (Visible)
		{
			neighborCollector.GetComponent<SphereCollider>().radius = magnetismRadius;
			Vector3 originalCursorPos = transform.position;
			// Move cursor with acceleration.
			Vector2 stickDeflection = Vector2.ClampMagnitude(
				GameModel.Model.GetPlayer(playerId).Controller.LeftAnalog 
				+ GameModel.Model.GetPlayer(playerId).Controller.RightAnalog
				+ GameModel.Model.GetPlayer(playerId).Controller.DPad, 1.0f);
			float stickDeflectionMagnitude = Mathf.Clamp(stickDeflection.magnitude, 0.0f, 1.0f);
			Vector3 moveDirection = new Vector3(stickDeflection.x, 0.0f, stickDeflection.y);
			GetComponent<CharacterController>().Move(moveDirection * (maxCursorSpeed * Time.deltaTime));
			
			// Calculate relative angle of cursor movement.
			float cursorMoveRelAngle = Vector3.Angle(originalCursorPos, transform.position);
			
			// Apply cursor magnetism.
			if (stickDeflectionMagnitude > GameModel.Model.GetPlayer(playerId).Controller.DeadZoneRadius)
			{
				float weightedAvgNum = 0.0f;
				float weightedAvgDenom = 0.0f;
				LinkedList<AbstractFishController> deadNeighbors = new LinkedList<AbstractFishController>();
				foreach (AbstractFishController neighbor in neighborCollector.Neighbors)
				{
					if(neighbor == null || neighbor.gameObject == null || neighbor.Alive)
					{
						deadNeighbors.AddLast(neighbor);
					}
					else if(neighbor is PreyController)
					{
						continue;
					}
					// Calculate relative angle.
					float neighborRelAngle = Vector3.Angle(transform.position, neighbor.Position);
					// Determine alignment of object angle and cursor move angle in 0-1 range.
					float normalizedNeighborAngle = neighborRelAngle - cursorMoveRelAngle;
					if (normalizedNeighborAngle > 180.0f)
					{
						normalizedNeighborAngle -= 360.0f;
					}
					else if (normalizedNeighborAngle < -180.0f)
					{
						normalizedNeighborAngle += 360.0f;
					}
					float alignmentPercent = (1.0f - (Mathf.Abs(normalizedNeighborAngle) / 180.0f));
					weightedAvgNum += normalizedNeighborAngle * alignmentPercent;
					weightedAvgDenom += alignmentPercent;
				}
				// Remove dead fish.
				foreach(AbstractFishController deadNeighbor in deadNeighbors)
				{
					neighborCollector.Neighbors.Remove(deadNeighbor);
				}
				// Move the cursor according to the magnetism.
				float magnetismRelativeAngle = (weightedAvgDenom > 0.0f ? weightedAvgNum / weightedAvgDenom : 0.0f);
				float magnetismWeight = Mathf.Clamp((1.0f - stickDeflectionMagnitude) * maxMagnetismStrength, 
					minMagnetismStrength, maxMagnetismStrength);

				float magneticAttractionDist = (maxCursorSpeed * Time.deltaTime) * magnetismWeight;
				if(magnetismRelativeAngle != 0.0f)
				{
					Quaternion originalRotation = transform.rotation;
					transform.eulerAngles = new Vector3(0.0f, cursorMoveRelAngle, 0.0f);
					transform.Rotate(new Vector3(0.0f, magnetismRelativeAngle, 0.0f));
					transform.position += Vector3.forward * magneticAttractionDist;
					transform.rotation = originalRotation;
				}
            }
			// Keep cursor on XZ plane (constant Y).
			Vector3 updatedPosition = transform.position;
			updatedPosition.y = yPosition;
			transform.position = updatedPosition;
		}
	}
}
