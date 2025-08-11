using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{
    public Vector2 MoveInput => _frameInput.Move;
    public bool IsGrounded => _isGrounded;

    public static Action OnJump;
    public static Action OnDash;

    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _coyoteTime = 0.1f;
    [SerializeField] private bool _spawnAtStartPoint = true;
    [SerializeField] private float _dashCooldown = 1f;

    private float _coyoteTimer;
    private bool _doubleJumpAvailable = false;
    private bool _controlEnabled = true;
    private float _dashTimer = 0f;

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private PlayerMovement _movement;
    private Collider2D _isGrounded;
    private Vector3 _defaultScenePlacement;

    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _defaultScenePlacement = transform.position;
    }

    private void Start()
    {
        var startpoint = FindAnyObjectByType<StartPoint>();
        if (_spawnAtStartPoint)
        {
            transform.position = startpoint.transform.position;
        }
        else
        {
            transform.position = _defaultScenePlacement;
        }
    }

    private void OnEnable()
    {
        OnJump += StartJump;
        OnDash += StartDashCooldown;
        EndPoint.OnExitReached += DisableControl;
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        OnJump -= StartJump;
        OnDash -= StartDashCooldown;
        EndPoint.OnExitReached -= DisableControl;
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void Update()
    {
        GatherInput();
        Movement();
        _isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
        CoyoteTimer();
        HandleJump();
        HandleDash();
        HandleSpriteFlip();
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

    private void GatherInput()
    {
        if (_controlEnabled)
        {
            _frameInput = _playerInput.FrameInput;
        }
        else
        {
            _frameInput = default;
        }
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

    private void StartJump()
    {
        _coyoteTimer = 0f;
    }

    private void CoyoteTimer()
    {
        if (CheckGrounded())
        {
            _coyoteTimer = _coyoteTime;
            // _doubleJumpAvailable = true;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void HandleDash()
    {
        if (_dashTimer > 0f)
        {
            _dashTimer -= Time.deltaTime;
        }
        if (!_frameInput.Dash) { return; }
        if (_dashTimer <= 0f)
        {
            OnDash?.Invoke();
        }
    }

    private void StartDashCooldown()
    {
        _dashTimer = _dashCooldown;
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

    private void DisableControl()
    {
        _controlEnabled = false;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        _controlEnabled = true;
    }

    public void StartTNTKnockback()
    {
        _movement.StartTNTKnockback();
    }
}
