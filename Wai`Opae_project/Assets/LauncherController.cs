using UnityEngine;
using System.Collections;

public class LauncherController : MonoBehaviour {

	public GameObject projectile;
	public Transform target;
	public Transform launchPosition;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;
    public int playerNumber;
    private Renderer spear;
    private Renderer launcher;
    private Renderer cursor;
    private float x, y, z;

    protected GameObject loadedProjectile = null;
	private float lastProjectileLaunch = 0.0f;

	// Use this for initialization
	void Start () {
		loadedProjectile = LoadProjectile();
        setLauncherVisibility();
        getTargetVisibility();
	}
	
	// Update is called once per frame
	void Update () {

        if(cursor.isVisible)
        {
            launcher.enabled = true;
            spear.enabled = true;
        }
        else if(!cursor.isVisible)
        {
            launcher.enabled = false;
            spear.enabled = false;
        }

        if (launcher.enabled == true)
        {
            if (playerNumber == 1)
            {
                if (Input.GetButtonDown("RightBumper_P1") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                }
                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            else if (playerNumber == 2)
            {
                if (Input.GetButtonDown("RightBumper_P2") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                }
                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            else if (playerNumber == 3)
            {
                if (Input.GetButtonDown("RightBumper_P3") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                }
                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            else if (playerNumber == 4)
            {
                if (Input.GetButtonDown("RightBumper_P4") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                }
                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }
        }
    }

    void setLauncherVisibility()
    {
        launcher = launchPosition.GetComponent<Renderer>();
        spear = projectile.GetComponent<Renderer>();
        launcher.enabled = true;
        spear.enabled = true;
    }

    void getTargetVisibility()
    {
        cursor = target.GetComponent<Renderer>();
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
