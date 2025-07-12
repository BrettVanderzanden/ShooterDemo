using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 MoveInput => _frameInput.Move;

    public static Action OnJump;

    public static PlayerController Instance;

    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jumpStrength = 14f;
    [SerializeField] private float _extraGravity = 1000f;
    [SerializeField] private float _gravityDelay = 0.25f;
    [SerializeField] private float _coyoteTime = 0.1f;
    [SerializeField] private float _maxFallSpeedVelocity = -25f;

    private float _timeInAir;
    private float _coyoteTimer;
    private bool _doubleJumpAvailable;

    private Coroutine _jetpackCoroutine;
    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Movement _movement;
    private Rigidbody2D _rigidBody;
    private Collider2D _isGrounded;

    public void Awake()
    {
        if (Instance == null) { Instance = this; }

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<Movement>();
    }

    private void OnEnable()
    {
        OnJump += ApplyJumpForce;
    }

    private void OnDisable()
    {
        OnJump -= ApplyJumpForce;
    }

    private void Update()
    {
        GatherInput();
        Movement();
        _isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
        CoyoteTimer();
        HandleJump();
        GravityDelay();
        HandleSpriteFlip();
    }

    private void FixedUpdate()
    {
        ExtraGravity();
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    public bool CheckGrounded()
    {
        //Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
        return _isGrounded;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    private void GravityDelay()
    {
        if (!CheckGrounded())
        {
            _timeInAir += Time.deltaTime;
        }
        else
        {
            _timeInAir = 0f;
        }
    }

    private void ExtraGravity()
    {
        if (_timeInAir > _gravityDelay)
        {
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime));
            if (_rigidBody.linearVelocityY < _maxFallSpeedVelocity)
            {
                _rigidBody.linearVelocityY = _maxFallSpeedVelocity;
            }
        }
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    private void Movement()
    {
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    private void HandleJump()
    {
        if (!_frameInput.Jump) { return; }

        if (CheckGrounded())
        {
            OnJump?.Invoke();
        }
        else if (_coyoteTimer > 0f)
        {
            OnJump?.Invoke();
        }
        else if (_doubleJumpAvailable)
        {
            _doubleJumpAvailable = false;
            OnJump?.Invoke();
        }
    }

    private void CoyoteTimer()
    {
        if (CheckGrounded())
        {
            _coyoteTimer = _coyoteTime;
            _doubleJumpAvailable = true;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void ApplyJumpForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        _timeInAir = 0f;
        _coyoteTimer = 0f;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    private void HandleSpriteFlip()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
}
