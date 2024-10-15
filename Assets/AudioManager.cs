using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----Audio Source------")]
    [SerializeField] AudioSource musicSource; // For background music
    [SerializeField] AudioSource sfxSource;   // For sound effects (SFX)

    [Header("-----Audio Clips------")]
    public AudioClip background;     // Background music clip
    public AudioClip ZombieDeath1;   // SFX for zombie death
    public AudioClip Shoot;          // SFX for shooting
    public AudioClip swordHit;
    public AudioClip spearHit;
    public AudioClip axeHit;

    private void Start()
    {
        // Play background music at the start
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        // Play the passed sound effect clip using the sfxSource
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip); // PlayOneShot so it doesn't interrupt other sounds
        }
    }

    public void Audio(string weaponType)
    {
        switch (weaponType)
        {
            case "Sword":
                PlaySFX(swordHit);
                break;
            case "Axe":
                PlaySFX(axeHit);
                break;
            case "Spear":
                PlaySFX(spearHit);
                break;
        }
    }
}