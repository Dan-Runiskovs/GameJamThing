using UnityEngine;
using UnityEngine.UIElements;

public class VehicleArrivalScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Objects")]
    [SerializeField] private GameObject _ramp;

    [Header("Sequence")]
    [SerializeField] private Vector3 _startPos = new Vector3();
    [SerializeField] private Vector3 _endPos = new Vector3();
    [SerializeField] private float _arrivalTime = 1.0f;

    [Space]
    [SerializeField] private Vector3 _endRotRamp = new Vector3();
    [SerializeField] private float _rampSpeed = 2.0f;
    [SerializeField] private GridBehaviour _gridBehaviour;

    private float _timer = 0f;
    private Vector3 _diffPos;
    private Quaternion _targetRot;

    int phase = 0;
    void Start()
    {
        enabled = true;
        transform.position = _startPos;
        _diffPos = _endPos-_startPos;
        _targetRot = Quaternion.Euler(_endRotRamp);

    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 0)
        {
            if (_timer < _arrivalTime)
            {
                _timer += Time.deltaTime;
                float percent = _timer / _arrivalTime;
                transform.position = _startPos + _diffPos * percent;
            }
            else
            {
                phase = 1;
                _timer = 0f;
            }
        }
        else if (phase == 1)
        {
            _gridBehaviour.transform.parent = null;
            _gridBehaviour?.StartGrid();
            phase = 2;


        }
        else if (phase == 2) 
        {
            _ramp.transform.rotation = Quaternion.Lerp(_ramp.transform.rotation, _targetRot, _rampSpeed * Time.deltaTime);
            if (_ramp.transform.rotation == _targetRot)
            {
                phase = 3;
                enabled = false;
            }
        }

    }
}
