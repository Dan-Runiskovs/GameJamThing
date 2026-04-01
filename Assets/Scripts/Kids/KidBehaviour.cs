using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class KidBehaviour : MonoBehaviour
{
    [Header("Needs")]
    [SerializeField] private List<BaseStand> _stands = new List<BaseStand>();
    private ItemScript.ItemTypes _itemWanted;

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

    private void Awake()
    {
        // --- Make kid visually happy ---
        foreach (var part in this.GetComponentsInChildren<MeshRenderer>())
        {
            if (part == null) continue;

            part.material = _happyMaterial;
        }
    }
    void Update()
    {
        if (_isSad) return;

        MoveUpdate();

        _happiness -= _happinessLossPS * Time.deltaTime;

        if(_happiness <= 0.0f) MakeSad();
    }

    private void MakeSad()
    {
        // --- set values ----
        _happiness = 0.0f;
        _isSad = true;

        // --- determine item wanted ---
        int randIdx = Random.Range(0, _stands.Count);
        _itemWanted = _stands.ElementAt(randIdx).StandItemType;
        Debug.Log("I want: " + _itemWanted.ToString());

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

}
