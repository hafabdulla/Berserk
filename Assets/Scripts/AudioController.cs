using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip plasmaGun;
    public AudioClip flyby;
    
    public void playPlasmaGunSound()
    {
        audiosource.PlayOneShot(plasmaGun);
    }
}
