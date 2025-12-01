using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip plasmaGun;
    public AudioClip flyby;
    public float flybyInterval = 10f; // interval in seconds
    void Start()
    {
        StartCoroutine(PlayFlybyEvery10Seconds());
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
}
