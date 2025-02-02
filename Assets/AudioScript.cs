using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public List<AudioClip> LandingAudioClips = new List<AudioClip>();
    public AudioClip WanderingAudioClip;
    public AudioClip FightingAudioClip;
    private AudioSource audioSource;

    public bool fight;
    public bool wandering;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        setWandering();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void setWandering()
    {
        wandering = true;
        audioSource.clip = WanderingAudioClip;
        audioSource.pitch = -0.5f;
        audioSource.Play();
    }
}
