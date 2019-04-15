using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionShooterSFX : MonoBehaviour
{

    [SerializeField] List<AudioClip> destructionSounds, shootingSounds;

    public void playDestructionSound()
    {
        //GetComponent<AudioSource>().PlayOneShot(destructionSounds[Random.Range(0, destructionSounds.Count)]);
    }

    public void playShootingSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(shootingSounds[Random.Range(0, shootingSounds.Count)]);
    }
}
