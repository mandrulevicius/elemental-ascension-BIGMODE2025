using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ProceduralAnimation : MonoBehaviour
{
    [SerializeField] LayerMask preyLayer;
    GameObject _player;

    [SerializeField] GameObject body;

    // maybe make a struct for each leg
    [SerializeField] List<GameObject> thighHips;
    [SerializeField] List<GameObject> legTargets;
    [SerializeField]  List<GameObject> windUpTargets ;

    List<Vector3> _localRestPositions = new();
    List<Vector3> _snapPositions = new();
    List<Vector3> _rayDirections = new();
    List<Ray> _rays = new();
    Ray _tempRay;
    RaycastHit _hit;

    Ray _groundRay;
    Ray _frontRay;

    [FormerlySerializedAs("maxRayDistance")] [SerializeField]
    float maxLegReach = 2;

    [SerializeField] LayerMask layersToHit;

    Vector3 _movementDirection = Vector3.zero;

    [SerializeField] float distanceToSnap = 1;
    [SerializeField] float legTargetVariation = 0.1f;
    [SerializeField] List<float> legStartOffsets = new() { 0f, 0.5f, 1f, -0.5f };

    public float _tick;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float windUpTime = 20;
    [SerializeField] private float attackTime = 10;
    [SerializeField] private float windDownTime = 15;
    [SerializeField] private float cooldown = 60;
    [SerializeField] float speed = 5f;
    [SerializeField] private float legMovespeed = 60f;
    [SerializeField] private float legLiftSpeed = 0.02f;
    public bool isCooldowning;
    private float movementTick;
    public bool windUp;
    public bool legChange;
    public bool isWindingDown;
    public bool reset;

    void Start()
    {
        _player = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .FirstOrDefault(obj => ((1 << obj.layer) & preyLayer) != 0);

        _groundRay = new Ray(body.transform.position, Vector3.down);
        _frontRay = new Ray(body.transform.position, Vector3.forward);
        InitializeLegs();
    }

    void InitializeLegs()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            _localRestPositions.Add(legTargets[i].transform.localPosition);
            legTargets[i].transform.position += new Vector3(0, 0, legStartOffsets[i]);
            _snapPositions.Add(legTargets[i].transform.position);
            Vector3 rayDirection = (transform.TransformPoint(_localRestPositions[i]) - thighHips[i].transform.position)
                .normalized;
            _rayDirections.Add(rayDirection);
            _rays.Add(new Ray(thighHips[i].transform.position, rayDirection));
        }
    }

    public void FixedUpdate()
    {
        movementTick += 1;
        if (isCooldowning) Tick();
        if (isCooldowning) return;
        HuntPlayer();
        if (isCooldowning) return;
        for (int i = 0; i < legTargets.Count; i++)
        {
            ProcessLeg(i);
        }

        // stay above ground
        _groundRay.origin = body.transform.position;
        _groundRay.direction = Vector3.down;
        Debug.DrawRay(_groundRay.origin, _groundRay.direction, Color.red);
        if (Physics.Raycast(_groundRay, out _hit, maxLegReach, layersToHit))
        {
            transform.position = _hit.point;
        }

        _frontRay.origin = body.transform.position;
        _frontRay.direction = _movementDirection;
        Debug.DrawRay(_frontRay.origin, _frontRay.direction, Color.red);
        if (Physics.Raycast(_frontRay, out _hit, maxLegReach, layersToHit))
        {
            // transform.LookAt(new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z)); // delta time here
        }
        // look at front leg snap positions?
    }


    void Tick()
    {
        _tick += 1;
        if (_tick >= windUpTime && windUp)
        {
            _tick = 0;
            windUp = false;
            isAttacking = true;
        }
        if (_tick >= attackTime && isAttacking)
        {
            _tick = 0;
            isAttacking = false;
          
            isWindingDown = true;
        }
        if (_tick >= windDownTime && isWindingDown)
        {
            for (int i = 0; i < legTargets.Count; i++)
            {
                ProcessLeg(i);
            }
            isWindingDown = false;
            reset = true;
            _tick = 0;
        }

        if (_tick >= cooldown && reset)
        {
            reset = false;
            isCooldowning = false;  
            _tick = 0;
            movementTick = 0;
            legChange = true;
        }
    }

    void HuntPlayer()
    {
        _movementDirection = (_player.transform.position - transform.position).normalized;
        transform.position += _movementDirection * (speed * Time.fixedDeltaTime);
        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));

        var distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (distanceToPlayer <= maxLegReach)
        {
            if (Physics.Raycast(_frontRay, out _hit, maxLegReach, layersToHit))
            {
                // transform.LookAt(new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z)); // delta time here
                
                _snapPositions[0] = _hit.point+_movementDirection;
                _snapPositions[1] = _hit.point;
                windUp = true;
                _tick = 0;
                isCooldowning = true;

                // isAttacking = true;
            }
        }
    }

    void ProcessLeg(int i)
    {
        // variable positions, dancing, whatever
        // Vector3 adjustedRayDirection = _rayDirections[i] + transform.rotation.eulerAngles;
        // Vector3 adjustedRayDirection = _rayDirections[i];

        // readjust rays according to current position
        _tempRay = _rays[i];
        _tempRay.origin = thighHips[i].transform.position;
        _rayDirections[i] = (transform.TransformPoint(_localRestPositions[i]) - _tempRay.origin).normalized;
        _tempRay.direction = _rayDirections[i];
        _rays[i] = _tempRay;

        Debug.DrawRay(_rays[i].origin, _rayDirections[i], Color.red);
        if (!Physics.Raycast(_rays[i], out _hit, maxLegReach, layersToHit))
        {
            // if raycast miss, try closer to center
            if (!Physics.Raycast(_rays[i].origin, (_rayDirections[i] + Vector3.down).normalized, out _hit, maxLegReach,
                    layersToHit))
            {
                if (!Physics.Raycast(_rays[i].origin, Vector3.down, out _hit, maxLegReach, layersToHit)) return;
            }
        }

        // if too far from resting position, move
        if (!(Vector3.Distance(_snapPositions[i], _hit.point) > distanceToSnap)) return;
        _snapPositions[i] = _hit.point + _movementDirection * Random.Range(0f, 1f) +
                            new Vector3(
                                Random.Range(-legTargetVariation, legTargetVariation),
                                0,
                                Random.Range(-legTargetVariation, legTargetVariation));


        // _snapPositions[i] += Vector3.Lerp(_snapPositions[i], _hit.point, Time.fixedDeltaTime * legSpeed);
    }

    private void LerpingTheLeg()
    {
        
    }
    void LateUpdate()
    {
        // snap legs
        for (int i = 0; i < legTargets.Count; i++)
        {
            if (windUp && i < windUpTargets.Count) 
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, windUpTargets[i].transform.position, _tick/windUpTime);
            }
            else if(isAttacking)
            {
             legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i], _tick/attackTime);
            }
            else if (isWindingDown)
            {
             legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i], _tick/windDownTime);
            }   
            else if (reset)
            {
             legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i], _tick/cooldown);
            }
            else
            {
             legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i], movementTick/legMovespeed);
            }
            
            
            if (movementTick >= legMovespeed)
            {
                movementTick = 0;
            }
        }
    }
}

// good enough for now, but fix climbing hills - IK on body? (i think wonky body rotation is interfering)

// fix legs too synced. - good enough for now?