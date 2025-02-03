using UnityEditor;
using UnityEngine;

public class PlantActions : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private LayerMask _particleLayer;
    private int _playerLayer;
    private GameObject player;
    private float _tick;
    private float _spawnTick;
    private float _growthTick;
    [SerializeField] private float spawnTime = 600f;
    [SerializeField] private float castRange = 0.4f;
    [SerializeField] private float attackSpeed = 5f;
    private bool _attack;
    private Vector3 _hit;
    private Vector3 _projectile;
    private GameObject _lastSpawn;

    private Vector3 _fullyGrownScale;
    [SerializeField] int ticksUntilFullyGrown = 120;
    [SerializeField] bool fullyGrown;
    [SerializeField] private int particleCapacity=2;
    [SerializeField] private GameObject spwanPlace;
     private Vector3 spawnPosition;
     private Vector3 startScale;
     public GameObject prefab;
     public bool spawned;

     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startScale = transform.localScale;
        _fullyGrownScale = transform.localScale;
        transform.localScale = Vector3.zero;
        _playerLayer = LayerMask.GetMask("Player");
        
        Collider [] playerCollider = Physics.OverlapSphere(  spwanPlace.transform.position, castRange*2, _playerLayer);
        if (playerCollider.Length > 0)
        {
            player = playerCollider[0].gameObject;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float randomRadius = Random.Range(0.1f, castRange);
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
             spawnPosition =
                spwanPlace.transform.position  + randomDirection * randomRadius;
     
        
        Collider[] particles = Physics.OverlapSphere(  spwanPlace.transform.position, castRange*2, _particleLayer);

        _tick += 1;
        if(fullyGrown && spawned)
        _spawnTick += 1;
        // spwan
        if (_spawnTick >= spawnTime)
        {
            if (particles.Length < particleCapacity)
            {
                _lastSpawn = Instantiate(particle, spawnPosition, Quaternion.identity);
                var coef = startScale.x / _lastSpawn.transform.localScale.x;
                _lastSpawn.transform.localScale =  transform.localScale / coef;
                if(player)
                {
                    _lastSpawn.transform.GetComponent<ProceduralAnimation>().range *=
                        player.GetComponent<EntityStats>().MultiplicativeModifier;
                    
                    Debug.Log(_lastSpawn.transform.GetComponent<ProceduralAnimation>().range);
                    Debug.Log(player.GetComponent<EntityStats>().MultiplicativeModifier);
                }
            }

            _spawnTick = 0;
        }

        if (_tick >= attackSpeed)
        {
            _tick = 0;
            if (particles.Length <= 0) return;

            _attack = true;
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

            // _hit = enemyColliders[0].transform.position;
            // _projectile = particles[0].transform.position;
        }
        // if (_attack )
        // {
        //     _projectile = Vector3.Lerp(_hit,_projectile, _tick / attackSpeed);
        //     if (_tick >= attackSpeed)
        //     {
        //         _tick = 0;
        //         _attack = false;
        //     }
        // }
        // findEnemy
        // findPlayer
        // move

        if (!fullyGrown)
        {
            _growthTick += 1;
            if (_growthTick >= ticksUntilFullyGrown)
            {
                fullyGrown = true;
            }

            transform.localScale += _fullyGrownScale / ticksUntilFullyGrown;
        }

        ;
    }

    void LateUpdate()
    {
    }

    public void Reset()
    {
     
        _growthTick = 0;
        fullyGrown = false;
        transform.localScale = Vector3.zero;
       
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, castRange);
    }
}