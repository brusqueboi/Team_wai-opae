using UnityEngine;
using System.Collections;

public class LauncherController : MonoBehaviour {

	public GameObject projectile;
	public Transform target;
	public Transform launchPosition;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;

	protected GameObject loadedProjectile = null;
	private float lastProjectileLaunch = 0.0f;

	// Use this for initialization
	void Start () {
		loadedProjectile = LoadProjectile();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("AButton") && Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay 
			&& loadedProjectile != null)
		{
			loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
			lastProjectileLaunch = Time.realtimeSinceStartup;
			loadedProjectile = null;
			StartCoroutine(LauncherReload());
		}
		else if(loadedProjectile != null)
		{
			ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
			loadedProjectile.transform.LookAt(target);
			loadedProjectile.transform.Rotate(loadedController.rotationOffset);
		}
	}

	public GameObject LoadProjectile()
	{
		GameObject projectileClone =
				(GameObject)Instantiate(projectile, launchPosition.position, launchPosition.rotation);
		projectileClone.transform.position = launchPosition.position;
		projectileClone.transform.LookAt(target);
		projectileClone.transform.Rotate(projectileClone.GetComponent<ProjectileController>().rotationOffset);
		return projectileClone;
	}

	private IEnumerator LauncherReload()
	{
		yield return new WaitForSeconds(reloadDelay);
		loadedProjectile = LoadProjectile();
	}
}
