using System.Collections.Generic;
using Unity.IntegerTime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ProceduralAnimation : MonoBehaviour
{
    
    [SerializeField] GameObject player;
    [SerializeField] float speed = 5f;
    Vector3 _movementDirection = Vector3.zero;
    
    [SerializeField] GameObject body;
    
    // maybe make a struct for each leg
    [SerializeField] List<GameObject> thighHips;
    [SerializeField] List<GameObject> legTargets;

    List<Vector3> _localRestPositions = new ();
    List<Vector3> _snapPositions = new ();
    List<Vector3> _rayDirections = new ();
    List<Ray> _rays = new ();
    Ray _tempRay;
    RaycastHit _hit;

    Ray _groundRay;
    
    [SerializeField] float maxRayDistance = 2;
    [SerializeField] LayerMask layersToHit;
    
    [SerializeField] float distanceToSnap = 1;
    [SerializeField] float legTargetVariation = 0.1f;
    [SerializeField] List<float> legStartOffsets = new() { 0f, 0.5f, 1f, -0.5f };

    float _tick;

    void Start()
    {
        InitializeLegs();
    }
    
    public void FixedUpdate()
    {
        
        _groundRay = new Ray(body.transform.position, Vector3.down);
        Tick();
        HuntPlayer();
        
        for (int i = 0; i < legTargets.Count; i++)
        {
            ProcessLeg(i);
        }
    }

    void InitializeLegs()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            _localRestPositions.Add(legTargets[i].transform.localPosition);
            legTargets[i].transform.position += new Vector3(0, 0, legStartOffsets[i]);
            _snapPositions.Add(legTargets[i].transform.position);
            Vector3 rayDirection = (transform.TransformPoint(_localRestPositions[i]) - thighHips[i].transform.position).normalized;
            _rayDirections.Add(rayDirection);
            _rays.Add(new Ray(thighHips[i].transform.position, rayDirection));
        }
    }

    void Tick()
    {
        _tick += 1;
        if (_tick >= 60)
        {
            _tick = 0;
            // number = Random.Range(-0.1f, 0.1f);
            //  + new Vector3(number, number, number)
        }
    }

    void HuntPlayer()
    {
        _movementDirection = (player.transform.position - transform.position).normalized;
        transform.position += _movementDirection * (speed * Time.fixedDeltaTime);
        transform.LookAt(transform.position + _movementDirection);
    }
    
    void ProcessLeg(int i)
    {
        // variable positions, dancing, whatever
        // Vector3 adjustedRayDirection = _rayDirections[i] + transform.rotation.eulerAngles;
        // Vector3 adjustedRayDirection = _rayDirections[i];

        _tempRay = _rays[i];
        _tempRay.origin = thighHips[i].transform.position;
        _rayDirections[i] = (transform.TransformPoint(_localRestPositions[i]) - _tempRay.origin).normalized;
        _tempRay.direction = _rayDirections[i];
        _rays[i] = _tempRay;
        
        Debug.DrawRay(_rays[i].origin, _rayDirections[i], Color.red);
        if (!Physics.Raycast(_rays[i], out _hit, maxRayDistance, layersToHit)) return;
        

        if (!(Vector3.Distance(_snapPositions[i], _hit.point) > distanceToSnap)) return;
        _snapPositions[i] = _hit.point + _movementDirection + new Vector3(
            Random.Range(-legTargetVariation, legTargetVariation),
            0,
            Random.Range(-legTargetVariation, legTargetVariation));
            
        // _snapPositions[i] += Vector3.Lerp(_snapPositions[i], _hit.point, Time.fixedDeltaTime * legSpeed);
    }
    
    void LateUpdate()
    {
        _groundRay.origin = body.transform.position;    
        Debug.DrawRay(_groundRay.origin, _groundRay.direction, Color.red);
        if (Physics.Raycast(_groundRay, out _hit, maxRayDistance, layersToHit))
        {
            transform.position = _hit.point;
        }
        
        for (int i = 0; i < legTargets.Count; i++)
        {
            legTargets[i].transform.position = _snapPositions[i];
        }
    }
}
