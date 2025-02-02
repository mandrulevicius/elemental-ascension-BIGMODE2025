using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class PlayerActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    EntityStats stats;
    StarterAssetsInputs _inputs;
    [SerializeField] private GameObject plant;
    [SerializeField] private GameObject CinemachineCamera;
    private RaycastHit _hit;
    [SerializeField] private float castRange = 10f;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private LayerMask whatIsPlant;
    public List<GameObject> plantPool = new List<GameObject>();
    [SerializeField] private GameObject plantPrefab;
    public event Action<List <GameObject>> OnPlantsChanged;

    private void OnEnable()
    {
        stats = GetComponent<EntityStats>();
        _inputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        Action();
        Growing();
        Taking();
    }

    public void Action()
    {
        if (_inputs.action)
        {
            _inputs.action = false;
            {
                if (plantPool.Count > 0)
                    if (Physics.Raycast(
                            new Ray(CinemachineCamera.transform.position, CinemachineCamera.transform.forward),
                            out _hit, castRange, layersToHit))
                        GetPlant(_hit.point, Quaternion.identity);
                OnPlantsChanged?.Invoke(plantPool);
            }
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

    public void Taking()
    {
        if (_inputs.taking)
        {
            _inputs.taking = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, castRange, whatIsPlant);

            for (int i = 0; i < colliders.Length; i++)
            {
                var prefabGameObject = colliders[i].gameObject;
                HidePlant(prefabGameObject.gameObject);
            }
        OnPlantsChanged?.Invoke(plantPool);
        }
    }

    void HidePlant(GameObject plant)
    {
        plant.SetActive(false);
        plantPool.Add(plant); // Store for reuse
    }

    // When you need it again:
    public GameObject GetPlant(Vector3 position, Quaternion rotation)
    {
        if (plantPool.Count > 0)
        {
            GameObject plant = plantPool[0];
            plantPool.RemoveAt(0);
            plant.transform.position = position;
            plant.transform.rotation = rotation;
            plant.SetActive(true); // Reactivate the plant
            plant.GetComponent<PlantActions>().Reset();
            return plant;
        }

        // No available plants, instantiate a new one
        return Instantiate(plantPrefab, position, rotation);
    }
}