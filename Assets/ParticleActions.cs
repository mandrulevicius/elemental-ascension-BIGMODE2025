using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ParticleActions : MonoBehaviour
{
    [SerializeField] private LayerMask _enemies;
    [SerializeField] public float range= 5f;
    [SerializeField] public float speed= 5f;
    [SerializeField] public float damage = 1f;
    [SerializeField] public int lifetimeTicks = 500;
    int _lifetimeTick;
    
        private float _distastanceToEnemy;
        int _tick;
        private bool enemyInRange;
        [SerializeField] int ticksUntilFullyGrown = 120;

        Vector3 _startPosition;
        [SerializeField] float growthVibration = 0.01f;
        private Vector3 _fullyGrownScale;
        [SerializeField] bool fullyGrown;

        [SerializeField] LayerMask friends;
        EntityStats entityStats;

        void Start()
    {
        
        entityStats = GetComponent<EntityStats>();
        for (int j = 0; j < 32; j++) if ((friends.value & (1 << j)) != 0) Physics.IgnoreLayerCollision(gameObject.layer, j, true);  
        _startPosition = transform.position;
        _fullyGrownScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
        

    private void OnTriggerEnter(Collider other)
    {
        EntityStats stats = other.gameObject.GetComponent<EntityStats>();
        if (stats)
        {
            stats.Health -= damage;
        } else
        {
            stats = other.gameObject.GetComponentInParent<EntityStats>();
            if (stats) stats.Health -= damage;
        }
        entityStats.Health -= damage;
    }

    void FixedUpdate()
    {
        _tick += 1;
        _lifetimeTick += 1;
        if (_lifetimeTick >= lifetimeTicks)
        {
            Destroy(gameObject, 1f);
        }
        else
        {
            if (fullyGrown) transform.localScale -= _fullyGrownScale / lifetimeTicks;
        }
        
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
