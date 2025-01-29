using System.Collections.Generic;
using Unity.IntegerTime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ProceduralAnimation : MonoBehaviour
{
    // maybe make a struct for each leg
    [SerializeField] GameObject player;
    [SerializeField] float speed = 5f;

    [SerializeField] List<GameObject> thighHips;
    [SerializeField] List<GameObject> legTargets;

    List<Vector3> _localRestPositions = new ();
    List<Vector3> _snapPositions = new ();
    List<Vector3> _rayDirections = new ();
    List<Ray> _rays = new ();
    Ray _tempRay;
    RaycastHit _hit;
    
    [SerializeField] float maxRayDistance = 2;
    [SerializeField] LayerMask layersToHit;
    
    [SerializeField] float distanceToSnap = 1;
    // [SerializeField] float legSpeed = 1;
    [SerializeField] float legTargetVariation = 0.1f;
    [SerializeField] List<float> legStartOffsets = new() { 0f, 0.5f, 1f, -0.5f };
    
    // [SerializeField] private float number;
    float _avgGroundPos;

    float _tick;

    void Start()
    {
        InitializeLegs();
    }
    
    public void FixedUpdate()
    {
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
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * (speed * Time.fixedDeltaTime);
        transform.LookAt(transform.position + direction);
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
        _snapPositions[i] = _hit.point + new Vector3(
            Random.Range(-legTargetVariation, legTargetVariation),
            0,
            Random.Range(-legTargetVariation, legTargetVariation));
            
        // _snapPositions[i] += Vector3.Lerp(_snapPositions[i], _hit.point, Time.fixedDeltaTime * legSpeed);
    }
    
    void LateUpdate()
    {
        _avgGroundPos = 0;
        for (int i = 0; i < legTargets.Count; i++)
        {
            legTargets[i].transform.position = _snapPositions[i];
            _avgGroundPos += legTargets[i].transform.position.y;
        }

        _avgGroundPos /= legTargets.Count;
        // transform.position = new Vector3(transform.position.x, (_avgGroundPos - transform.position.y) * speed * Time.deltaTime, transform.position.z);
        // transform.position = new Vector3(transform.position.x, _avgGroundPos, transform.position.z);
    }
}
