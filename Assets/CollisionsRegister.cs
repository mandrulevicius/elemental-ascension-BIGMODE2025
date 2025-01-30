using System;
using UnityEngine;

public class CollisionsRegister : MonoBehaviour
{
    EntityStats playerStats; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    private void OnTriggerEnter(Collider other)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer != playerLayer) return;
       playerStats =  other.gameObject.GetComponent<EntityStats>();
       playerStats.Health -=1;
    }

 
}
