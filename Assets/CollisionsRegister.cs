using System;
using UnityEngine;

public class CollisionsRegister : MonoBehaviour
{
    EntityStats playerStats; 
    public AudioClip HitAudioClip;
    public ParticleSystem HitParticles;
    public GameObject HitGlowBall;

    private void OnTriggerEnter(Collider other)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer != playerLayer)
        {
            return;
        }
       playerStats =  other.gameObject.GetComponent<EntityStats>();
        if(HitAudioClip)
               AudioSource.PlayClipAtPoint(HitAudioClip, transform.position, 1);
        if (HitParticles)
            Instantiate(HitParticles, transform.position, Quaternion.identity);
        if (HitGlowBall)
        {
            var glowBall = Instantiate(HitGlowBall, transform.position, Quaternion.identity);
            Destroy(glowBall, 0.5f);
        }
        
        playerStats.Health -= gameObject.GetComponent<EntityStats>().damage;
    }

 
}
