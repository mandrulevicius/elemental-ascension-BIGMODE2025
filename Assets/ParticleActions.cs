using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ParticleActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private LayerMask _enemies;
    [SerializeField] public float range= 5f;
    [SerializeField] public float speed= 5f;
    [SerializeField] public float damage = 1f;
    
        private float _distastanceToEnemy;
        int _tick;
        private bool enemyInRange;
        [SerializeField] int ticksUntilFullyGrown = 120;

        private Vector3 _fullyGrownScale;
        Vector3 _startPosition;
        [SerializeField] float growthVibration = 0.01f;
        [SerializeField] bool fullyGrown;

        [SerializeField] LayerMask friends;
        
        void Start()
    {
        
        for (int j = 0; j < 32; j++) if ((friends.value & (1 << j)) != 0) Physics.IgnoreLayerCollision(gameObject.layer, j, true);  
        _fullyGrownScale = transform.localScale;
        _startPosition = transform.position;
        transform.localScale = Vector3.zero;
        // add this to plants too
    }
        

    private void OnCollisionEnter(Collision other)
    {

        EntityStats stats = other.gameObject.GetComponent<EntityStats>();
        if (stats != null)
        {
            stats.Health -= damage;
        }
        GetComponent<EntityStats>().Health -= damage;
        // should also explode when hits anything other than itself?
    }

    void FixedUpdate()
    {
        _tick += 1;
        
        
        if (_tick >= ticksUntilFullyGrown)
        {
            _tick = 0;
        }
        if (fullyGrown) return;
        if (_tick == 0)
        {
            fullyGrown = true;
        }
        transform.localScale += _fullyGrownScale / ticksUntilFullyGrown;
    }

    void LateUpdate()
    {
        if (fullyGrown) return;
        transform.position = _startPosition + new Vector3(
            Random.Range(-growthVibration, growthVibration), 
            Random.Range(-growthVibration, growthVibration), 
            Random.Range(-growthVibration, growthVibration));
        _startPosition = transform.position;
    }
    
}
