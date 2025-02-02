using System;
using UnityEngine;

public class SpawnArenaCreeps : MonoBehaviour
{
    [SerializeField]GameObject arenaCreeps;
    [SerializeField] LayerMask playerLayer;
    private void OnTriggerEnter(Collider other)
    {
        int player = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer == player)
        {
        Debug.Log(other.gameObject.name);
            arenaCreeps.SetActive(true);
        }
    }
}
