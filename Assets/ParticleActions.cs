using System;
using UnityEngine;

public class ParticleActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private LayerMask _enemies;
    [SerializeField] public float range= 5f;
    [SerializeField] public float speed= 5f;
    [SerializeField] public float damage = 1f;
    
        private float _distastanceToEnemy;
        private int _tick;
        private bool enemyInRange;

        void Start()
    {
            
    }
        

    private void OnCollisionEnter(Collision other)
    {

        EntityStats stats = other.gameObject.GetComponent<EntityStats>();
        if (stats != null)
        {
            stats.Health -= damage;
            GetComponent<EntityStats>().Health -= damage;
                
        }
        // should also explode when hits anything other than itself?
    }

    
}
