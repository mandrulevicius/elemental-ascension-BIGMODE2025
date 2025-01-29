using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    // maybe make a struct for each leg

    [SerializeField] private List<GameObject> thighHips;
    [SerializeField] private List<GameObject> legTargets;
    
    private List<Vector3> _localRestPositions = new ();
    private List<Vector3> _snapPositions = new ();
    private List<Vector3> _rayDirections = new ();
    private List<Ray> _rays = new ();
    private Ray _tempRay;
    private RaycastHit _hit;
    
    [SerializeField] private float maxDistance = 2;
    [SerializeField] private LayerMask layersToHit;
    
    
    [SerializeField] private float distanceToSnap = 1;
    [SerializeField] private float legSpeed = 1;
    
    [SerializeField] private float number;

    private float _tick;

    private void Start()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            _localRestPositions.Add(legTargets[i].transform.localPosition);
            _snapPositions.Add(legTargets[i].transform.position);
            Vector3 rayDirection = (transform.TransformPoint(_localRestPositions[i]) - thighHips[i].transform.position).normalized;
            _rayDirections.Add(rayDirection);
            _rays.Add(new Ray(thighHips[i].transform.position, rayDirection));
        }
    }

    private void ProcessLeg(int i)
    {
        _tick += 1;
        if (_tick >= 60)
        {
            _tick = 0;
            number = Random.Range(0f, 1f);
            //  + new Vector3(number, number, number)
        }
        // variable positions, dancing, whatever
        // Vector3 adjustedRayDirection = _rayDirections[i] + transform.rotation.eulerAngles;
        // Vector3 adjustedRayDirection = _rayDirections[i];

        _tempRay = _rays[i];
        _tempRay.origin = thighHips[i].transform.position;
        _rayDirections[i] = (transform.TransformPoint(_localRestPositions[i]) - _tempRay.origin).normalized;
        _tempRay.direction = _rayDirections[i];
        _rays[i] = _tempRay;
        
        Debug.DrawRay(_rays[i].origin, _rayDirections[i], Color.red);
        if (!Physics.Raycast(_rays[i], out _hit, maxDistance, layersToHit)) return;
        
        if (Vector3.Distance(_snapPositions[i], _hit.point) > distanceToSnap)
        {
            _snapPositions[i] = _hit.point;
            // _snapPositions[i] += Vector3.Lerp(_snapPositions[i], _hit.point, Time.fixedDeltaTime * legSpeed);
        }
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            ProcessLeg(i);
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
