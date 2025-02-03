using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawnArenaCreeps : MonoBehaviour
{
    [SerializeField]GameObject arenaCreeps;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool hordeSpawn = false;
    [SerializeField] float hordeSpawnTime = 50*60f;
    private void OnTriggerEnter(Collider other)
    {
        int player = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer == player)
        {
            if (hordeSpawn)
            {
              var spawnedArena=  Instantiate(arenaCreeps, transform.position, Quaternion.identity);
              spawnedArena.SetActive(true);
              return;
            }
            arenaCreeps.SetActive(true);
        }
    }

   void FixedUpdate()
    {
        
    }
}
