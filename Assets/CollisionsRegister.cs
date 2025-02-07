using System;
using UnityEngine;

public class CollisionsRegister : MonoBehaviour
{
    EntityStats otherStats; 
    public AudioClip HitAudioClip;
    public ParticleSystem HitParticles;
    public GameObject HitGlowBall;

    private void OnTriggerEnter(Collider other)
    {
        // if (gameObject.layer == other.gameObject.layer) return;

        if (gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int friendsLayer = LayerMask.NameToLayer("Friends");
            if (other.gameObject.layer != playerLayer || other.gameObject.layer != friendsLayer)
            {
                return;
            }
            otherStats = other.gameObject.GetComponentInParent<EntityStats>();
            if (!otherStats) return;
            otherStats.Health -= otherStats.damage;
            if(HitAudioClip)
            {
                
                AudioSource.PlayClipAtPoint(HitAudioClip, transform.position, 0.4f);
                
            }
            if (HitParticles)
                Instantiate(HitParticles, transform.position, Quaternion.identity);
            if (HitGlowBall)
            {
                var glowBall = Instantiate(HitGlowBall, transform.position, Quaternion.identity);
                Destroy(glowBall, 0.5f);
            }
        }
        
        if (gameObject.layer == LayerMask.NameToLayer("Player") || gameObject.layer == LayerMask.NameToLayer("Friends"))
        {
            int enemiesLayer = LayerMask.NameToLayer("Enemies");
            if (other.gameObject.layer != enemiesLayer)
            {
                return;
            }
            otherStats = other.gameObject.GetComponentInParent<EntityStats>();
            if (!otherStats) return;
            otherStats.Health -= otherStats.damage;
            if(HitAudioClip)
            {
                
                AudioSource.PlayClipAtPoint(HitAudioClip, transform.position, 0.4f);
                
            }
            if (HitParticles)
                Instantiate(HitParticles, transform.position, Quaternion.identity);
            if (HitGlowBall)
            {
                var glowBall = Instantiate(HitGlowBall, transform.position, Quaternion.identity);
                Destroy(glowBall, 0.5f);
            }
        }
        
        
    }

 
}
