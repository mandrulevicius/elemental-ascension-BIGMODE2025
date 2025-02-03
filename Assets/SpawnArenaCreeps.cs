using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawnArenaCreeps : MonoBehaviour
{
    [SerializeField] GameObject arenaCreeps;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool hordeSpawn = false;
    [SerializeField] float hordeSpawnTime = 50 * 60f;
    [SerializeField] private bool onTimer = false;
    [SerializeField] private float _timer = 0f;
    [SerializeField] bool _start  = false;

    private void OnTriggerEnter(Collider other)
    {
        int player = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer == player)
        {
            if (!hordeSpawn)
            {
                arenaCreeps.SetActive(true);
                return;
            }

            if (onTimer)
            {
                _start  = true;
                var spawnedArenaFirstForHorde = Instantiate(arenaCreeps, transform.position, Quaternion.identity);
                spawnedArenaFirstForHorde.SetActive(true);
                return;
            }
            var spawnedArena = Instantiate(arenaCreeps, transform.position, Quaternion.identity);
            spawnedArena.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if(!onTimer&& !_start){return;}
        _timer += 1;
        if (_timer >= hordeSpawnTime)
        {
            _timer = 0;
            var spawnedArena = Instantiate(arenaCreeps, transform.position, Quaternion.identity);
            spawnedArena.SetActive(true);
        }
    }
}