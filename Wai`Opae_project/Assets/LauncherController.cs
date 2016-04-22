using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LauncherController : MonoBehaviour {

	public GameObject projectile;
	public Transform target;
	public Transform cameraPosition;
	public Vector3 cameraOffset;
    public AudioClip shoot;
    public AudioClip splash;
    public Transform launchPosition;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;
    public float delay = 1;
    public float vol = .3f;
    public float svol = .7f;
	public int playerId = 1;
	public float multiplayerSpreadDist = 2.0f;

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
				loadedProjectile.GetComponent<Renderer>().enabled = visible;
			}
		}
	}

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

		if (GameModel.Model.GetPlayer(playerId).Controller.RightBumper 
			&& Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay 
			&& loadedProjectile != null)
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

		if (Visible)
		{
			int enabledPlayers = 0;
			int thisEnabledPlayersIdx = -1;
			for (int i = 0; i < GameModel.Model.Players.Length; i++)
			{
				if (GameModel.Model.Players[i] != null && GameModel.Model.Players[i].Enabled)
				{
					if (GameModel.Model.Players[i].PlayerIndex == playerId)
					{
						thisEnabledPlayersIdx = enabledPlayers;
					}
					enabledPlayers++;
				}
			}

			thisEnabledPlayersIdx += 1;
			enabledPlayers += (enabledPlayers > 3 ? 0 : 1);
			float positionPercent = (float)thisEnabledPlayersIdx / (float)enabledPlayers;
			
			Vector3 updatedPos = cameraPosition.position + cameraOffset;
			updatedPos.x -= multiplayerSpreadDist / 2.0f;
			updatedPos.x += multiplayerSpreadDist * (positionPercent);
			gameObject.transform.position = updatedPos;
		}

		if(loadedProjectile != null)
		{
			loadedProjectile.transform.position = gameObject.transform.position;
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
