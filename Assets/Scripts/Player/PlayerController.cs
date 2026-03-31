using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActionMap _playerActionMap;
    private Rigidbody _rb;

    [Header("Aiming")]
    [SerializeField] private float _aimSpeed = 8f;
    [SerializeField] private float _minAim = 0.7f;  //Minimum joystick input to register aiming
    [Tooltip("Player, right joystick input.")]
    private Vector2 _aimInputDirection;
    [Tooltip("Final direction the player will look towards.")]
    private Vector3 _lookDirection = Vector3.zero;

    [Header("Moving")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _minMove = 0.05f;    //Prevents very slight joycon drift
    [SerializeField] private float _backwardMovementMultiplier = 0.7f;  //The movement multiplier for aiming backwards
    [SerializeField] private float _moveAcceleration = 4f;  //Acceleration, especially noticeable when you quickly turn 180°
    [Tooltip("Player, left joystick input.")]
    private Vector2 _moveInputDirection;
    [Tooltip("Final direction the player will move towards.")]
    private Vector3 _moveDirection;


    private bool _MovementEnabled = true;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        PlayerInput input = GetComponent<PlayerInput>();
        _playerActionMap = input.actions.FindActionMap("Player");

        _playerActionMap.FindAction("Move").performed += context => _moveInputDirection = context.ReadValue<Vector2>();
        _playerActionMap.FindAction("Move").canceled += context => _moveInputDirection = Vector2.zero;
        _playerActionMap.FindAction("Look").performed += context => _aimInputDirection = context.ReadValue<Vector2>();
        _playerActionMap.FindAction("Look").canceled += context => _aimInputDirection = Vector2.zero;
    }

    private void OnEnable() => _playerActionMap.Enable();
    private void OnDisable() => _playerActionMap.Disable();

    private void Update()
    {
        HandleAiming();
    }

    private void FixedUpdate()
    {
        HandleMoving();
    }

    //Handles the aiming input
    private void HandleAiming()
    {
        //If the aiming input is registered, aiming decides player rotation
        if (_aimInputDirection.magnitude >= _minAim)
        {
            _lookDirection = new Vector3(_aimInputDirection.x, 0f, _aimInputDirection.y).normalized;
            RotatePlayer();
        }
        //Otherwise if the player is moving, movement decides player rotation
        else if (_moveInputDirection.magnitude >= _minMove)
        {
            _lookDirection = new Vector3(_moveInputDirection.x, 0f, _moveInputDirection.y).normalized;
            RotatePlayer();
        }
    }

    //Rotates the player towards their current _lookDirection
    private void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
        _rb.MoveRotation(Quaternion.Lerp(
            _rb.rotation,
            targetRotation,
            _aimSpeed * Time.deltaTime
        ));
    }

    //Handles the player movement
    private void HandleMoving()
    {

        if (!_MovementEnabled) return;
        float acceleration = _moveAcceleration;
        //When a player stops moving, they don't slide as much
        if (_moveInputDirection.magnitude < _minMove)
        {
            acceleration = _moveAcceleration * 2;
            _moveInputDirection = Vector2.zero;
        }

        _moveDirection = Vector3.MoveTowards(_moveDirection, new Vector3(_moveInputDirection.x, 0f, _moveInputDirection.y), acceleration * Time.fixedDeltaTime);

        //Check which way the player is aiming compared to where they are moving, and returns a value between 0-1
        float alignment01 = (Vector3.Dot(_moveDirection.normalized, _lookDirection.normalized) + 1f) * 0.5f;
        float speedMultiplier = Mathf.Lerp(_backwardMovementMultiplier, 1f, alignment01);

        Vector3 movement = _moveDirection * (_moveSpeed * speedMultiplier) * Time.fixedDeltaTime;

        _rb.MovePosition(_rb.position + movement);
    }

    public void EnableMovement(bool enabled) { _MovementEnabled = enabled; }
}
