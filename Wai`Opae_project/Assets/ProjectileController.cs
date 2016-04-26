using UnityEngine;
using System.Collections;
using System;
using Assets;

public class ProjectileController : MonoBehaviour
{
	public event EventHandler<LauncherController.ProjectileEventArgs> CollisionHit;
	public event EventHandler<LauncherController.ProjectileEventArgs> CollisionMiss;

	public float maxVelocity = 10.0f;
	public float acceleration = 5.0f;
	public float velocity = 0.0f;
	public float despawnDelay = 2.0f;
	public Vector3 rotationOffset = Vector3.zero;
	public float despawnDistance = 1000.0f; // 1 Km

	private Quaternion forwardRotation;
	private Quaternion offsetRotation;
	private Vector3 launchPosition;
	private bool projectileFired = false;

	void Start()
	{
		gameObject.transform.Rotate(rotationOffset);
    }

    void Update()
    {
		if(velocity > 0.0f)
		{
			if(velocity < maxVelocity)
			{
				velocity = Mathf.Clamp(velocity + (acceleration * Time.deltaTime), 0.0f, maxVelocity);
			}
			gameObject.transform.rotation = forwardRotation;
			gameObject.transform.position += gameObject.transform.forward * velocity * Time.deltaTime;
			gameObject.transform.rotation = offsetRotation;
			if(Vector3.Distance(launchPosition, gameObject.transform.position) > despawnDistance)
			{
				velocity = 0.0f;
				FireCollisionMiss();
				Destroy(gameObject);
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
				if (hitFish is PreyController)
				{
					FireCollisionHit();
				}
				else if (hitFish is RoiController)
				{
					FireCollisionHit();
				}
			}
			StartCoroutine(ProjectileDespawn());
		}
	}

	public void FireProjectile(Transform target)
	{
		projectileFired = true;
		launchPosition = gameObject.transform.position;
		gameObject.transform.LookAt(target);
		forwardRotation = gameObject.transform.rotation; // Save forward rotation.
		gameObject.transform.Rotate(rotationOffset);
		offsetRotation = gameObject.transform.rotation; // Save offset rotation.
		velocity = acceleration * Time.deltaTime;
    }

	private IEnumerator ProjectileDespawn()
	{
		yield return new WaitForSeconds(despawnDelay);
		Destroy(gameObject);
	}

	private void FireCollisionHit()
	{
		Debug.Log("Hit");
		if(CollisionHit != null)
		{
			CollisionHit.Invoke(this, new LauncherController.ProjectileEventArgs(this));
		}
	}

	private void FireCollisionMiss()
	{
		Debug.Log("Miss");
		if (CollisionMiss != null)
		{
			CollisionMiss.Invoke(this, new LauncherController.ProjectileEventArgs(this));
		}
	}
}
