using UnityEditor;
using UnityEngine;

public class PlantActions : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    private int _particleLayer;
    private int _playerLayer;
    private int _enemyLayer;

    private float _tick;
    private float _spawnTick;
    [SerializeField] private float spawnTime = 600f;
    [SerializeField] private float castRange = 20f;
    [SerializeField] private float attackSpeed = 5f;
    private bool attack;
    private float distastanceToEnemy;
    private Vector3 _hit;
    private Vector3 _projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _particleLayer = LayerMask.GetMask("Particles");
        _playerLayer = LayerMask.GetMask("Player");
        _enemyLayer = LayerMask.GetMask("Enemies");
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] particles = Physics.OverlapSphere(transform.position, castRange, _particleLayer);
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, castRange, _enemyLayer);

        _tick += 1;
        _spawnTick += 1;
        // spwan
        if (_spawnTick >= spawnTime)
        {
            if (particles.Length < 10)
            {
                float randomRadius = Random.Range(1f, castRange);
                Vector3 randomDirection = Random.insideUnitSphere.normalized;
                Vector3 spawnPosition = transform.position + randomDirection * randomRadius; // Offset by radius
                Instantiate(particle, spawnPosition, Quaternion.identity);
            }

            _spawnTick = 0;
        }

        if (_tick >= attackSpeed)
        {
            _tick = 0;
            if (particles.Length <= 0)return;
            if (enemyColliders.Length <= 0) return;
            attack = true;
                // for (int i = 0; i < enemyColliders.Length; i++)
                // {
                //     if (particles.Length > 0)
                //         for (int j = 0; j < particles.Length; j++)
                //         {
                //             if (distastanceToEnemy < Vector3.Distance(particles[j].transform.position,
                //                     enemyColliders[i].transform.position))
                //             {
                //                 distastanceToEnemy = Vector3.Distance(particles[j].transform.position,
                //                     enemyColliders[i].transform.position);
                //             }
                //
                //         }
                // }
            
                _hit = enemyColliders[0].transform.position;
                _projectile = particles[0].transform.position;
        }
        if (attack )
        {
            _projectile = Vector3.Lerp(_hit,_projectile, _tick / attackSpeed);
            if (_tick >= attackSpeed)
            {
                _tick = 0;
                attack = false;
            }
        }
        // findEnemy
        // findPlayer
        // move
    }

    void LateUpdate()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, castRange);
    }
}