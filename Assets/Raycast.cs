using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hit;
    public List<GameObject> legTargets;
    [SerializeField] private float maxDistance = 2;
    [SerializeField] private LayerMask layersToHit;
    private List<Vector3> _snapPositions = new ();
    private List<Vector3> _rayDirections = new ();
    [SerializeField] private float distanceToSnap = 1;
    [SerializeField] private float number;

    private float _tick;

    private void Start()
    {
        foreach (GameObject legTarget in legTargets)
        {
            _snapPositions.Add(legTarget.transform.position);
            _rayDirections.Add((legTarget.transform.position - transform.position).normalized);
        }
    }

    private void CheckForColliders(int i)
    {
        _tick += 1;
        if (_tick >= 60)
        {
            _tick = 0;
            number = Random.Range(0f, 1f);
            //  + new Vector3(number, number, number)
        }


        // Vector3 adjustedRayDirection = _rayDirections[i] + transform.rotation.eulerAngles;
        Vector3 adjustedRayDirection = _rayDirections[i];
        _ray = new Ray(transform.position, adjustedRayDirection.normalized);

        if (!Physics.Raycast(_ray, out _hit, maxDistance, layersToHit)) return;
        Debug.DrawRay(_ray.origin, _rayDirections[i], Color.red);
        if (Vector3.Distance(legTargets[i].transform.position, _hit.point) > distanceToSnap)
        {
            _snapPositions[i] = _hit.point;
            // _snapPositions[i]= Vector3.Lerp(legTargets[i].transform.position, _hit.point, Time.deltaTime * 1);

        }
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            CheckForColliders(i);
        }
        

    }

    private void LateUpdate()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {

            legTargets[i].transform.position = _snapPositions[i];
        }
    }
}
