using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    void Start()
    {
        if (gameObject.GetComponent<ParticleSystem>())
        {
            Destroy(gameObject, gameObject.GetComponent<ParticleSystem>().main.duration - 0.1f);
        }
        else
        {
            Destroy(gameObject, 0f);
        }
        
    }       
}
