using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public float vol = .3f;
    public float evol = .3f;
    public float tvol = .3f;
    public AudioClip shoot;
    public AudioClip hit;
    public AudioClip eat;
    public AudioClip background;
    public AudioClip timer;
    public LauncherController LC;
    public AudioSource source { get { return GetComponent<AudioSource>(); } }
    private AudioSource tsource { get { return GetComponent<AudioSource>(); } }

    // Use this for initialization
    public void Start() {

        gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        tsource.playOnAwake = false;
    }

	// Update is called once per frame
	public void Update () {
	
	}

    public void PlaySound()
    {
        source.PlayOneShot(shoot, vol);
    }

    public void playTimerSound()
    {
        tsource.PlayOneShot(timer, tvol);
    }

    public void playFishEatenSound()
    {
        tsource.PlayOneShot(eat, evol);
    }

}