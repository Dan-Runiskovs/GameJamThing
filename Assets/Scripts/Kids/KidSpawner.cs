using UnityEngine;

public class KidSpawner : MonoBehaviour
{
    [SerializeField] private int _kidsToSpwn = 10;
    private int _kidsSpawned = 0;

    [SerializeField] private GameObject _kidPrefab;

    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private Transform _wanderZoneCenter;

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
            kid.GetComponent<KidBehaviour>().wanderZone.center = _wanderZoneCenter;
            _kidsSpawned++;

            if (_kidsSpawned == _kidsToSpwn) KidSpawned?.Invoke();
        }
    }
}
