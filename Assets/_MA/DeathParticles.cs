using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, gameObject.GetComponent<ParticleSystem>().main.duration);
    }       
}
