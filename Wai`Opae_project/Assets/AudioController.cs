using UnityEngine;
using System.Collections;

public class AudioController {

	// Use this for initialization
	public void Start () {
		GameModel.Model.PreyConsumed += (sender, args) =>
		{
			AudioSource roiAudioSource = args.RoiObject.GetComponent<AudioSource>();
			roiAudioSource.PlayOneShot(roiAudioSource.clip);
		};

		AddProjectileHitEvent(1);
		AddProjectileHitEvent(2);
		AddProjectileHitEvent(3);
		AddProjectileHitEvent(4);

		AddProjectileLaunchedEvent(1);
		AddProjectileLaunchedEvent(2);
		AddProjectileLaunchedEvent(3);
		AddProjectileLaunchedEvent(4);
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

	private void AddProjectileLaunchedEvent(int playerId)
	{
		GameModel.Model.GetPlayer(playerId).Launcher.ProjectileLaunched += (sender, args) =>
		{
			AudioSource launcherAudio = 
			GameModel.Model.GetPlayer(playerId).Launcher.gameObject.GetComponent<AudioSource>();
			launcherAudio.PlayOneShot(launcherAudio.clip);
		};
	}

	private void AddProjectileHitEvent(int playerId)
	{
		GameModel.Model.GetPlayer(playerId).Launcher.ProjectileHit += (sender, args) =>
		{
			AudioSource projectileAudio = args.Projectile.gameObject.GetComponent<AudioSource>();
			projectileAudio.PlayOneShot(projectileAudio.clip);
		};
	}
}
