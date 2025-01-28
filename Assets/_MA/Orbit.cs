using System;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // public float distanceToOrbit = 1000;
    public float speed = 1;
    
    void FixedUpdate()
    {
        // TODO FIX POSITION ORBITING
        // Vector3 position = transform.position;
        // Vector3 rotationEulerAngles = transform.eulerAngles;
        //
        // float stepCount = distanceToOrbit / 360;
        // float step = stepCount * speed;
        // // Debug.Log(rotationEulerAngles.x);
        // if (rotationEulerAngles.x < 180)
        // {
        //     position.x += step;
        //     position.y -= step;
        // }
        // else
        // {
        //     position.x -= step;
        //     position.y += step;
        // }
        // transform.position = position;
        
        transform.Rotate(speed * Time.fixedDeltaTime, 0f, 0f);
    }
}
