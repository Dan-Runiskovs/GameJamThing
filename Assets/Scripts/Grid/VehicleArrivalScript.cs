using TMPro;
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

    [Space]
    [SerializeField] private Vector3 _LeavePos = new Vector3();
    [Space]
    [SerializeField] private GameObject _prefab;

    private float _timer = 0f;
    private Vector3 _diffPos;
    private Vector3 _diffPosLeave;
    private Quaternion _targetRot;
    private Quaternion _targetLeaveRot = Quaternion.Euler(0f,0f,0f);
    private bool _SoundStarted = false;
    private bool _SoundStarted2 = false;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _audioSourceBrake;

    [SerializeField] AudioClip _StartaudioClip;
    [SerializeField] AudioClip _brakeaudioClip;
    [SerializeField] AudioClip _driveAwayaudioClip;
    public int phase = 0;
    void Start()
    {
        enabled = true;
        _timer = 0f;
        _SoundStarted = false;
        _SoundStarted2 = false;
        phase = 0;
        transform.position = _startPos;
        if (_gridBehaviour != null) _gridBehaviour = GetComponentInChildren<GridBehaviour>();
        _diffPos = _endPos-_startPos;
        _diffPosLeave = _LeavePos - _startPos;
        _targetRot = Quaternion.Euler(_endRotRamp);
        AudioSource audioPlayer = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 0)
        {
            if (_timer < _arrivalTime)
            {
                if (!_SoundStarted)
                {
                     _audioSource.PlayOneShot(_StartaudioClip);
                    _SoundStarted = true;
                }
                _timer += Time.deltaTime;
                float percent = _timer / _arrivalTime;
                transform.position = _startPos + _diffPos * percent;
            }
            else
            {

               _audioSourceBrake.PlayOneShot(_brakeaudioClip); 
                phase = 1;
                _timer = 0f;
            }
        }
        else if (phase == 1)
        {
            var parent = _gridBehaviour.transform.parent;
            _gridBehaviour.transform.parent = null;
            _gridBehaviour?.StartGrid();
            phase = 2;
                         _gridBehaviour.transform.SetParent(parent);


        }
        else if (phase == 2) 
        {
            _ramp.transform.rotation = Quaternion.Lerp(_ramp.transform.rotation, _targetRot, _rampSpeed * Time.deltaTime);
            if (_ramp.transform.rotation == _targetRot)
            {
                phase = 3;

            }
        }
        
        else if (phase == 4) 
        {
            if (!_SoundStarted2)
            {
                _audioSource.PlayOneShot(_driveAwayaudioClip);
                _SoundStarted2 = true;
            }
            _ramp.transform.rotation = Quaternion.Lerp(_ramp.transform.rotation, _targetLeaveRot, _rampSpeed * Time.deltaTime);
            if (_ramp.transform.rotation == _targetLeaveRot)
            {
                phase = 5;
                _timer = 0f;
            }
        }
        else if (phase == 5)
        {
            if (_timer < _arrivalTime)
            {
   
                _timer += Time.deltaTime;
                float percent = _timer / _arrivalTime;
                transform.position = _endPos + _diffPosLeave * percent;
            }
            else
            {
                phase = 6;
                //_timer = 0f;
                var GO = Instantiate(_prefab, _startPos, transform.rotation);
                //GO.GetComponent<VehicleArrivalScript>().Start();
                Destroy(gameObject);

            }
        }

    }
}
