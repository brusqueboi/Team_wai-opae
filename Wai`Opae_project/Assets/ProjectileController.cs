using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
	public Animation despawnAnimation;

	public float maxVelocity = 10.0f;
	public float acceleration = 5.0f;
	public float velocity = 0.0f;
	public float despawnDelay = 2.0f;
	public Vector3 rotationOffset = Vector3.zero;
	public float despawnDistance = 1000.0f; // 1 Km

	private Quaternion forwardRotation;
	private Quaternion offsetRotation;
	private Vector3 launchPosition;

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
				Destroy(gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		velocity = 0.0f;
		StartCoroutine(ProjectileDespawn());
	}

	public void FireProjectile(Transform target)
	{
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
		if(despawnAnimation != null)
		{
			despawnAnimation.Play();
			do
			{
				yield return null;
			} while (despawnAnimation.isPlaying);
		}
		Destroy(gameObject);
	} 
}
