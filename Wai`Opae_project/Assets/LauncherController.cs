using UnityEngine;
using System.Collections;

public class LauncherController : MonoBehaviour {

	public GameObject projectile;
	public Transform target;
    public AudioClip shoot;
    public AudioClip splash;
    public Transform launchPosition;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;
    public float delay = 1;
    public float vol = .3f;
    public float svol = .7f;

	protected GameObject loadedProjectile = null;
	private float lastProjectileLaunch = 0.0f;
    private AudioSource source { get { return GetComponent<AudioSource>(); }}

	// Use this for initialization
	void Start () {
		loadedProjectile = LoadProjectile();
        gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("RightBumper_P1") && Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay 
			&& loadedProjectile != null)
		{
            PlaySound();
            loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
			lastProjectileLaunch = Time.realtimeSinceStartup;
			loadedProjectile = null;
            StartCoroutine(LauncherReload());
            PlaySoundDelay();
           
        }
		else if(loadedProjectile != null)
		{
			ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
			loadedProjectile.transform.LookAt(target);
			loadedProjectile.transform.Rotate(loadedController.rotationOffset);
		}
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
