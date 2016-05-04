using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets;

public class LauncherController : MonoBehaviour {
	// Events
	public event EventHandler<ProjectileEventArgs> ProjectileLoaded;
	public event EventHandler<ProjectileEventArgs> ProjectileLaunched;
	public event EventHandler<ProjectileEventArgs> ProjectileHit;
	public event EventHandler<ProjectileEventArgs> ProjectileMiss;

	// Control
	public GameObject projectile;
	public Transform target;
	public float yPosition = 1.87f;
    public AudioClip shoot;
    public AudioClip splash;
	public float interstitialDelay = 1.0f;
	public float reloadDelay = 1.0f;
	public Vector3 reloadOffset;
	public float reloadDuration = 1.0f;

	// Audio
    public float delay = 1;
    public float vol = .3f;
    public float svol = .7f;
	public int playerId = 1;
	public float multiplayerSpreadDist = 2.0f;

	private float reloadStartTime;
	private Vector3 centerPosition;

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
		centerPosition = GameObject.Find("Main Camera").transform.position;
		centerPosition.y = yPosition;
		loadedProjectile = LoadProjectile();
        gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
		projectile.SetActive(false);
		loadedProjectile.SetActive(false);
		GameModel.Model.LevelChanged += (sender, args) =>
		{
			if(loadedProjectile != null)
			{
				loadedProjectile.SetActive(GameModel.Model.Level != 0);
			}
		};
		GameModel.Model.EndgameDetected += (sender, args) => 
		{
			if (loadedProjectile != null)
			{
				loadedProjectile.SetActive(false);
			}
		};
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!GameModel.Model.GameSuspended && !GameModel.Model.AnimationSuspended && GameModel.Model.Level != 0)
		{
			if (Utils.AnyButtonPressed(playerId, false) && Time.realtimeSinceStartup - lastProjectileLaunch > interstitialDelay
			&& loadedProjectile != null)
			{
				PlaySound();
				loadedProjectile.GetComponent<ProjectileController>().FireProjectile(target);
				lastProjectileLaunch = Time.realtimeSinceStartup;
				loadedProjectile = null;
				StartCoroutine(LauncherReload());
				PlaySoundDelay();

			}
			else if (loadedProjectile != null && loadedProjectile.transform.position != transform.position)
			{
				float animProgress = (Time.time - reloadStartTime) / reloadDuration * 10;

				loadedProjectile.transform.position =
					Vector3.Lerp((transform.position - reloadOffset), transform.position, animProgress);
			}
		}
		// Rotate projectile to look at target.
		if (loadedProjectile != null)
		{
			ProjectileController loadedController = loadedProjectile.GetComponent<ProjectileController>();
			loadedProjectile.transform.LookAt(target);
			loadedProjectile.transform.Rotate(loadedController.rotationOffset);
		}
		// Position launcher.
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

			Vector3 updatedPos = centerPosition;
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
		projectile.SetActive(true);
		GameObject projectileClone =
				(GameObject)Instantiate(projectile, transform.position - reloadOffset, transform.rotation);
		projectile.SetActive(false);
		projectileClone.transform.LookAt(target);
		projectileClone.transform.Rotate(projectileClone.GetComponent<ProjectileController>().rotationOffset);
		reloadStartTime = Time.time;
		return projectileClone;
	}

	private IEnumerator LauncherReload()
	{
		yield return new WaitForSeconds(reloadDelay - reloadDuration);
		loadedProjectile = LoadProjectile();
		ProjectileController controller = loadedProjectile.GetComponent<ProjectileController>();
		controller.CollisionHit += (sender, args) => FireProjectileHit(controller, args.Target);
		controller.CollisionMiss += (sender, args) => FireProjectileMiss(controller);
		FireProjectileLoaded(controller);
	}

	private void FireProjectileLoaded(ProjectileController projectile)
	{
		if (ProjectileLoaded != null)
		{
			ProjectileLoaded.Invoke(this, new ProjectileEventArgs(projectile, null));
		}
	}

	private void FireProjectileLaunched(ProjectileController projectile)
	{
		if (ProjectileLaunched != null)
		{
			ProjectileLaunched.Invoke(this, new ProjectileEventArgs(projectile, null));
		}
	}

	private void FireProjectileHit(ProjectileController projectile, AbstractFishController target)
	{
		if (ProjectileHit != null)
		{
			ProjectileHit.Invoke(this, new ProjectileEventArgs(projectile, target));
		}
	}

	private void FireProjectileMiss(ProjectileController projectile)
	{
		if (ProjectileMiss != null)
		{
			ProjectileMiss.Invoke(this, new ProjectileEventArgs(projectile, null));
		}
	}


	public class ProjectileEventArgs : EventArgs
	{
		public ProjectileController Projectile { get; private set; }
		public AbstractFishController Target { get; private set; }

		public ProjectileEventArgs(ProjectileController projectile, AbstractFishController target)
		{
			Projectile = projectile;
			Target = target;
		}
	}
}
