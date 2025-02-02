using System;
using UnityEngine;

public class CollisionsRegister : MonoBehaviour
{
    EntityStats playerStats; 
    public AudioClip HitAudioClip;
    public ParticleSystem HitParticles;

    private void OnTriggerEnter(Collider other)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer != playerLayer) return;
       playerStats =  other.gameObject.GetComponent<EntityStats>();
        if(HitAudioClip)
               AudioSource.PlayClipAtPoint(HitAudioClip, transform.position, 1);
        if (HitParticles)
            Instantiate(HitParticles, transform.position, Quaternion.identity);
        playerStats.Health -=1;
    }

 
}
