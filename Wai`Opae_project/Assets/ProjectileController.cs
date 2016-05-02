using UnityEngine;
using System.Collections;
using System;
using Assets;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour
{
	public event EventHandler<LauncherController.ProjectileEventArgs> CollisionHit;
	public event EventHandler<LauncherController.ProjectileEventArgs> CollisionMiss;

	public int playerId = 1;
	public float maxVelocity = 10.0f;
	public float acceleration = 5.0f;
	public float velocity = 0.0f;
	public Vector3 rotationOffset = Vector3.zero;
	public float despawnDistance = 1000.0f; // 1 Km
	public float aimAssistStrength = 1.0f;
	public float magnetismStrength = 1.0f;

	private Quaternion forwardRotation;
	private Quaternion offsetRotation;
	private Vector3 launchPosition;
	private bool projectileFired = false;
	private NeighborCollectionController neighborCollector;
	private RoiController target = null;

	void Start()
	{
		gameObject.transform.Rotate(rotationOffset);
		neighborCollector = GetComponent<NeighborCollectionController>();
		GameModel.Model.LevelChanged += (sender, args) =>
		{
			if(neighborCollector != null)
			{
				neighborCollector.Neighbors.Clear();
			}
			target = null;
		};
    }

    void Update()
    {
		if(velocity > 0.0f)
		{
			if (Vector3.Distance(launchPosition, gameObject.transform.position) > despawnDistance)
			{
				velocity = 0.0f;
				FireCollisionMiss();
				Destroy(gameObject);
				return;
			}
			if (velocity < maxVelocity)
			{
				velocity = Mathf.Clamp(velocity + (acceleration * Time.deltaTime), 0.0f, maxVelocity);
			}
			gameObject.transform.rotation = forwardRotation;
			gameObject.transform.position += gameObject.transform.forward * velocity * Time.deltaTime;
			gameObject.transform.rotation = offsetRotation;
			if (target != null && transform.InverseTransformPoint(target.Position).z > 1.5f)
			{
				Quaternion originalRotation = transform.rotation;
				transform.LookAt(target.Position);
				float rotationDifference = (Quaternion.Inverse(originalRotation) * transform.rotation).eulerAngles.y;
				if(rotationDifference > 180.0f)
				{
					rotationDifference -= 360.0f;
				}
				else if(rotationDifference < -180.0f)
				{
					rotationDifference += 360.0f;
				}
				transform.rotation = originalRotation;
				rotationDifference *= magnetismStrength;
				transform.Rotate(new Vector3(0.0f, rotationDifference, 0.0f));

				gameObject.transform.position += gameObject.transform.forward * velocity * Time.deltaTime;
			}
			else
			{
				gameObject.transform.position += gameObject.transform.forward * velocity * Time.deltaTime;
				gameObject.transform.rotation = offsetRotation;
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(projectileFired)
		{
			velocity = 0.0f;
			Debug.Log("Projectile Hit: " + collision.gameObject.name);
			AbstractFishController hitFish = Utils.GetAbstractFishController(collision.gameObject);
			if(hitFish != null)
			{
				hitFish.Hit(this);
				FireCollisionHit(hitFish);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public void FireProjectile(Transform target)
	{
		projectileFired = true;
		// Select the roi closest to the player cursor.
		RoiController nearestRoi = null;
		float nearestRoiDist = float.MaxValue;
		Vector3 cursorPosition = GameModel.Model.GetPlayer(playerId).Cursor.transform.position;
		List<AbstractFishController> deadNeighbors = new List<AbstractFishController>();
		foreach (AbstractFishController neighbor in neighborCollector.Neighbors)
		{
			if(!neighbor.Alive)
			{
				deadNeighbors.Add(neighbor);
				continue;
			}
			if (neighbor is RoiController)
			{
				float dist = Vector3.Distance(cursorPosition, neighbor.transform.position);
				if (dist < nearestRoiDist)
				{
					nearestRoi = neighbor as RoiController;
					nearestRoiDist = dist;
				}
			}
		}
		foreach(AbstractFishController deadNeighbor in deadNeighbors)
		{
			neighborCollector.Neighbors.Remove(deadNeighbor);
		}

		// Add aim assist.
		if(nearestRoi != null)
		{
			this.target = nearestRoi;
			// Calculate the flight time of projectile.
			float roiDist = Vector3.Distance(transform.position, nearestRoi.Position);
			float maxVelocityTime = maxVelocity / acceleration;
			float maxVelocityDist = maxVelocityTime * acceleration;
			float flightTime = maxVelocityTime;
			if(maxVelocityDist > roiDist)
			{
				float differencePercent = (maxVelocityDist - roiDist) / maxVelocityDist;
				flightTime -= maxVelocityTime * differencePercent;
			}
			else
			{
				float remainingDist = roiDist - maxVelocityDist;
				float remainingTime = remainingDist * maxVelocity;
				flightTime += remainingTime;
			}

			flightTime *= aimAssistStrength;
			Vector3 originalPostion = nearestRoi.Position;
			nearestRoi.Position += Vector3.forward * nearestRoi.Velocity * flightTime;
			transform.LookAt(nearestRoi.Position);
			nearestRoi.Position = originalPostion;
			//Debug.Log("Relative Rotation: " + relativeRotation);

		}

		launchPosition = gameObject.transform.position;
		gameObject.transform.LookAt(target);
		forwardRotation = gameObject.transform.rotation; // Save forward rotation.
		gameObject.transform.Rotate(rotationOffset);
		offsetRotation = gameObject.transform.rotation; // Save offset rotation.
		velocity = acceleration * Time.deltaTime;
    }

	private void FireCollisionHit(AbstractFishController hitFish)
	{
		if(CollisionHit != null)
		{
			CollisionHit.Invoke(this, new LauncherController.ProjectileEventArgs(this, hitFish));
		}
	}

	private void FireCollisionMiss()
	{
		if (CollisionMiss != null)
		{
			CollisionMiss.Invoke(this, new LauncherController.ProjectileEventArgs(this, null));
		}
	}
}
