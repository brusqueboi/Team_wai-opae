using UnityEngine;
using System.Collections;

public class LauncherController : MonoBehaviour {

	public GameObject projectile;
	public Transform target;
    public AudioClip shoot;
    public AudioClip splash;
    public Transform launchPosition;
    public int playerNumber;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;
    public float delay = 1;
    public float vol = .3f;
    public float svol = .7f;

	protected GameObject loadedProjectile = null;
	private float lastProjectileLaunch = 0.0f;
    private AudioSource source { get { return GetComponent<AudioSource>(); }}
    private Renderer spear;
    private Renderer launcher;
    private Renderer cursor;
    

    // Use this for initialization
    void Start () {
		loadedProjectile = LoadProjectile();
        gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        getTargetVisibility();
        setLauncherVisibility();
	}
	
	// Update is called once per frame
	void Update () {
        // reinitialize newly loaded projectile to spear (if removed, the spear cannot toggle visibility after first shot)
        spear = loadedProjectile.GetComponent<Renderer>();

        // shows/hides launcher and spear if player is not enabled
        launcher.enabled = cursor.isVisible;
        spear.enabled = cursor.isVisible;

        // if the launcher is shown
        if(launcher.isVisible)
        {
            // player 1 launcher
            if (playerNumber == 1)
            {
                if (Input.GetButtonDown("RightBumper_P1") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    PlaySound();
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                    PlaySoundDelay();
                }

                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            // player 2 launcher
            else if(playerNumber == 2)
            {
                if (Input.GetButtonDown("RightBumper_P2") &&
                         Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                         loadedProjectile != null)
                {
                    PlaySound();
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                    PlaySoundDelay();
                }

                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            // player 3 launcher
            else if(playerNumber == 3)
            {
                if (Input.GetButtonDown("RightBumper_P3") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    PlaySound();
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                    PlaySoundDelay();
                }

                else if (loadedProjectile != null)
                {
                    ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
                    loadedProjectile.transform.LookAt(target);
                    loadedProjectile.transform.Rotate(loadedController.rotationOffset);
                }
            }

            // player 4 launcher
            else if(playerNumber == 4)
            {
                if (Input.GetButtonDown("RightBumper_P4") &&
                    Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay &&
                    loadedProjectile != null)
                {
                    PlaySound();
                    loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
                    lastProjectileLaunch = Time.realtimeSinceStartup;
                    loadedProjectile = null;
                    StartCoroutine(LauncherReload());
                    PlaySoundDelay();
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
        // sets the launcher to allow visibility toggle
        launcher = launchPosition.GetComponent<Renderer>();
        // sets the first loaded projectile to allow visibility toggle
        spear = loadedProjectile.GetComponent<Renderer>();
    }

    void getTargetVisibility()
    {
        // get the player's cursor visibility
        cursor = target.GetComponent<Renderer>();
    }

    void PlaySound()
    {
        source.PlayOneShot(shoot, svol);
        
    }

    void PlaySoundDelay()
    {
        source.PlayDelayed(delay);
        source.PlayOneShot(splash,vol);
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
