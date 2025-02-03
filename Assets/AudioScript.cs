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
        if (wandering)
        {
        setWandering();
        }
        if (fight)
        {
            wandering = false;
            setFighting();
        }
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
   public void setFighting()
    {
        wandering = true;
        audioSource.clip = FightingAudioClip;
        audioSource.pitch = 0f;
        audioSource.Play();
    }
}
