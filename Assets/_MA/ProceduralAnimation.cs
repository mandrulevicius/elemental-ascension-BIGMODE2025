using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ProceduralAnimation : MonoBehaviour
{
    GameObject _pray;

    [SerializeField] GameObject body;

    // maybe make a struct for each leg
    [SerializeField] List<GameObject> thighHips;
    [SerializeField] List<GameObject> legTargets;
    [SerializeField] List<GameObject> windUpTargets;

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

    [SerializeField] LayerMask preyLayer;

    [FormerlySerializedAs("layersToHit")] [SerializeField]
    LayerMask whatIsGroud;

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
    [SerializeField] private float range = 5f;
    [SerializeField] private float legMovespeed = 60f;
    [SerializeField] private float legLiftSpeed = 0.02f;
    public bool isCooldowning;
    private float movementTick;
    public bool windUp;
    public bool legChange;
    public bool isWindingDown;
    public bool reset;
    [SerializeField] private int attackingLeg;
    private Collider[] other;
    private EntityStats stats;
    private float _deathTick;
    [SerializeField] private float dieTime = 240;

    void Start()
    {
        stats = GetComponent<EntityStats>();
        // _pray = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            // .FirstOrDefault(obj => ((1 << obj.layer) & preyLayer) != 0);

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
        if (stats.dead)
        {
            _deathTick += 1;
            return;
        }

        if (isCooldowning)
        {
            Tick();
            return;
        }

        movementTick += 1;

        // ParallelEnumerable.ForAll();
        for (int i = 0; i < legTargets.Count; i++)
        {
            ProcessLeg(i);
        }

        if (!_pray && movementTick % 20 == 0)
        {
            other = Physics.OverlapSphere(transform.position, range, preyLayer);

            if (other.Length > 0)
                for (int i = 0; i < other.Length; i++)
                {
                    if (other[i].transform.root.gameObject.GetComponent<EntityStats>().dead)
                    {
                        continue;
                    }

                    _pray = other[i].gameObject;
                    break;
                }
        }

        if (_pray)
        {
            HuntPlayer();
        }

        // stay above ground
        _groundRay.origin = body.transform.position;
    
        _groundRay.direction = new Vector3(0, 0, 0);
        if (Physics.Raycast(_groundRay, out _hit, maxLegReach, whatIsGroud))
        {
            transform.position = _hit.point;
        }

        _frontRay.origin = body.transform.position;
        _frontRay.direction = _movementDirection;
        if (Physics.Raycast(_frontRay, out _hit, maxLegReach, whatIsGroud))
        {   
            if(Vector3.Distance(transform.position, _hit.point) < 0.1 && legTargets.Count < 1);
            {
                stats.Health -= 1;
            }
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
        var distanceToPlayer = Vector3.Distance(_pray.transform.position, transform.position);
        Debug.Log(distanceToPlayer);
        Debug.Log(_pray.gameObject.tag);
        if (distanceToPlayer > range) _pray = null;
        if (!_pray) return;
        _movementDirection = (_pray.transform.position - transform.position).normalized;
        transform.position += _movementDirection * (speed * Time.fixedDeltaTime);

        if (distanceToPlayer > maxLegReach)
            transform.LookAt(new Vector3(_pray.transform.position.x, transform.position.y, _pray.transform.position.z));
        if (distanceToPlayer <= maxLegReach && _snapPositions.Count > 0)
        {
            if (Physics.Raycast(_frontRay, out _hit, maxLegReach, preyLayer))
            {
                // transform.LookAt(new Vector3(_hit.point.x, _hit.point.y + 1, _hit.point.z)); // delta time here
                if (Vector3.Distance(_snapPositions[0], _hit.point) < Vector3.Distance(_snapPositions[1], _hit.point))
                {
                    _snapPositions[0] = _hit.point;
                    attackingLeg = 0;
                }
                else
                {
                    attackingLeg = 1;
                    _snapPositions[1] = _hit.point;
                }

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
        if (!Physics.Raycast(_rays[i], out _hit, maxLegReach, whatIsGroud))
        {
            // if raycast miss, try closer to center
            if (!Physics.Raycast(_rays[i].origin, (_rayDirections[i] + Vector3.down).normalized, out _hit, maxLegReach,
                    whatIsGroud))
            {
                if (!Physics.Raycast(_rays[i].origin, Vector3.down, out _hit, maxLegReach, whatIsGroud)) return;
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


    void LateUpdate()
    {
        if (stats.dead)
        {
            if (_deathTick >= dieTime)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (!isCooldowning)
        {
            for (int i = 0; i < legTargets.Count; i++)
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i],
                    movementTick / legMovespeed);
            return;
        }

        // snap legs
        for (int i = 0; i < legTargets.Count; i++)
        {
            if (windUp && i == attackingLeg)
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position,
                    windUpTargets[i].transform.position, _tick / windUpTime);
            }
            else if (isAttacking)
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i],
                    _tick / attackTime);
            }
            else if (isWindingDown)
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i],
                    _tick / windDownTime);
            }
            else if (reset)
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i],
                    _tick / cooldown);
            }
            else
            {
                legTargets[i].transform.position = Vector3.Lerp(legTargets[i].transform.position, _snapPositions[i],
                    movementTick / legMovespeed);
            }
        }
    }
}

// good enough for now, but fix climbing hills - IK on body? (i think wonky body rotation is interfering)

// fix legs too synced. - good enough for now?