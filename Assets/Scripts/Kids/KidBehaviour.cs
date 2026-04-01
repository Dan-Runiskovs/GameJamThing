using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class KidBehaviour : MonoBehaviour
{
    [Header("Needs")]
    public List<BaseStand> stands = new List<BaseStand>();
    private ItemScript.ItemTypes _itemWanted;
    public ItemScript.ItemTypes ItemWanted
    {
        get { return _itemWanted; }
    }

    private float _nextSadRollTime = 0f;

    [Header("Movement")]
    public NavMeshAgent agent;
    [System.Serializable]
    public class WanderZone { public Transform center; public float radius = 8f; }
    public WanderZone wanderZone;
    [SerializeField] private float _minPauseAtPoint = 0.5f;
    [SerializeField] private float _maxPauseAtPoint = 2.0f;
    private float _pauseUntil;

    [Header("Happiness")]
    private float _happiness = 100.0f;
    public float Happiness
    {
        get { return _happiness; }
    }
    [SerializeField] private float _happinessLossPS = 10.0f;
    private bool _isSad = false;
    public bool IsSad
    {
        get { return _isSad; }
    }

    [SerializeField] private Material _sadMaterial;
    [SerializeField] private Material _happyMaterial;

    private bool _carried;
    private Collider _col;

    private void Awake()
    {
        _nextSadRollTime = Time.time + Random.Range(0f, 1f);
        // --- Make kid visually happy ---
        foreach (var part in this.GetComponentsInChildren<MeshRenderer>())
        {
            if (part == null) continue;

            part.material = _happyMaterial;
        }
    }

    private void Start()
    {
        _col = GetComponent<Collider>();
    }

    void Update()
    {
        if (_isSad)
        {
            _happiness -= _happinessLossPS * Time.deltaTime * 0.5f;
            _happiness = Mathf.Max(0.0f, _happiness); // clamp happiness at 0

            return;
        }
        MoveUpdate();

        _happiness -= _happinessLossPS * Time.deltaTime;

        if (_happiness <= 40.0f)
        {
            if (Time.time >= _nextSadRollTime)
            {
                RollSadness();
                _nextSadRollTime = Time.time + 1.0f; // roll once per second
            }
        }
    }

    public void Carry()
    {
        _carried = true;
        _col.enabled = false;
    }

    public void Uncarry()
    {
        _carried = false;
        _col.enabled = true;
    }

    private void RollSadness()
    {
        float sadChance = Mathf.InverseLerp(40f, 0f, _happiness) * 100f;

        float rand = Random.Range(0.0f, 100.0f);

        if (rand <= sadChance)
        {
            MakeSad();
        }
    }

    private void MakeSad()
    {
        // --- set values ----
        _isSad = true;

        StopAgent(true);

        // --- determine item wanted ---
        int randIdx = Random.Range(0, stands.Count);
        _itemWanted = stands.ElementAt(randIdx).StandItemType;
        Debug.Log("I want: " + _itemWanted.ToString());
        this.GetComponentInChildren<KidUIController>().Complain(ItemWanted);

        // --- Make kid visually sad ---
        foreach (var part in this.GetComponentsInChildren<MeshRenderer>())
        {
            if (part == null) continue;

            part.material = _sadMaterial;
        }
    }

    public void Satisfy(float extent = 100.0f)
    {
        // --- set values ---
        _isSad = false;
        _happiness = extent;

        StopAgent(false);
        this.GetComponentInChildren<KidUIController>().StopComplaining();

        // --- Make kid visually happy ---
        foreach (var part in this.GetComponentsInChildren<MeshRenderer>())
        {
            if (part == null) continue;

            part.material = _happyMaterial;
        }
    }

    private void MoveUpdate()
    {
        if (Time.time < _pauseUntil) return;

        if (!agent.pathPending &&
            (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.05f))
        {
            _pauseUntil = Time.time + Random.Range(_minPauseAtPoint, _maxPauseAtPoint);
            ChooseNextTarget();
            return;
        }
    }
    private void ChooseNextTarget()
    {
        if (wanderZone == null) return;

        Vector3 p = RandomPointOnNavmesh(wanderZone.center.position, wanderZone.radius, 12);
        agent.SetDestination(p);
        return;
    }

    private Vector3 RandomPointOnNavmesh(Vector3 center, float radius, int attempts)
    {
        for (int i = 0; i < attempts; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 c = center + new Vector3(r.x, 0f, r.y);
            if (NavMesh.SamplePosition(c, out var hit, 2f, NavMesh.AllAreas))
            {
                var path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    return hit.position;
            }
        }
        return center;
    }
    void StopAgent(bool stop)
    {
        agent.isStopped = stop;
        agent.updateRotation = !stop;
        if (stop) agent.velocity = Vector3.zero;
    }
}
