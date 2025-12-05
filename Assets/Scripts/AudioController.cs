using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip plasmaGun;
    public AudioClip flyby;
    public AudioClip landing;
    public AudioClip droidSound; //must be played in a loop. represents the breathing of the robot
    public float flybyInterval = 10f; // interval in seconds

    void Start()
    {
        StartCoroutine(PlayFlybyEvery10Seconds());
        PlayDroidLoop();
    }

    public void playPlasmaGunSound()
    {
        audiosource.PlayOneShot(plasmaGun);
    }

    IEnumerator PlayFlybyEvery10Seconds()
    {
        while (true)
        {
            playFlybySound();
            yield return new WaitForSeconds(flybyInterval); // wait 10 seconds
        }
    }

    public void playFlybySound()
    {
        audiosource.PlayOneShot(flyby);
    }

    public void PlayDroidLoop()
    {
        audiosource.loop = true;
        audiosource.clip = droidSound;
        audiosource.Play();
    }

    public void playLandingSound()
    {
        audiosource.PlayOneShot(landing);
    }
}
