using System;
using UnityEngine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class PlayerActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    EntityStats stats;
    StarterAssetsInputs _inputs;
    [SerializeField] private GameObject plant;
    [SerializeField] private GameObject CinemachineCamera;
    private RaycastHit _hit;
    [SerializeField] private float castRange= 10f;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private LayerMask whatIsPlant;

    private void OnEnable()
    {
        stats = GetComponent<EntityStats>();
        _inputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
       Action();
       Growing();
    }

    public void Action()
    {
        if (_inputs.action)
        {
            if(Physics.Raycast(new Ray(CinemachineCamera.transform.position, CinemachineCamera.transform.forward),
                   out _hit, castRange, layersToHit))
                _inputs.action = false;
            Instantiate(plant, _hit.point, Quaternion.identity);
        }
    }

    public void Growing()
    {
        if (_inputs.grow)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, castRange, whatIsPlant);
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
            }
        }
    }
}