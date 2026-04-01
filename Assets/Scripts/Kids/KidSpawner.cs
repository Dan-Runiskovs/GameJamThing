using System.Collections.Generic;
using UnityEngine;

public class KidSpawner : MonoBehaviour
{
    [SerializeField] private int _kidsToSpwn = 10;
    private int _kidsSpawned = 0;

    [SerializeField] private GameObject _kidPrefab;

    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private Transform _wanderZoneCenter;

    [SerializeField]
    private List<BaseStand> _stands = new List<BaseStand>();

    private float _internalTimer = 0.0f;

    public static event System.Action KidSpawned;

    void Update()
    {
        if (_kidsSpawned >= _kidsToSpwn) return;

        _internalTimer += Time.deltaTime;

        if( _internalTimer > 1.0f )
        {
            _internalTimer = 0.0f;
            
            GameObject kid = Instantiate<GameObject>(_kidPrefab, _spawnpoint.position, Quaternion.identity);
            KidBehaviour kb = kid.GetComponent<KidBehaviour>();

            kb.wanderZone.center = _wanderZoneCenter;
            kb.stands = _stands;

            _kidsSpawned++;

            if (_kidsSpawned == _kidsToSpwn) KidSpawned?.Invoke();
        }
    }
}
