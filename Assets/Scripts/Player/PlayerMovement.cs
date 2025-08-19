using UnityEngine;
using System;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpStrength = 14f;
    [SerializeField] private float _dashStrength = 14f;
    [SerializeField] private float _dashDuration = .3f;
    [SerializeField] private float _gravityDelay = 0.25f;
    [SerializeField] private float _extraGravity = 1000f;
    [SerializeField] private float _maxFallVelocity = -25f;
    [SerializeField] private float _tntKnockbackDuration = 0.2f;
    [SerializeField] private TrailRenderer _tntTrailRenderer;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _fallMultiplier = 2.5f;

    private float _moveX;
    private bool _canMove = true;
    private bool _dashing = false;
    private float _dashTime = 0f;
    private float _defaultGravityScale;
    private Vector2 _dashStartPt, _dashEndPt;
    private float _timeInAir;
    private bool _inTNTKnockback = false;

    private Rigidbody2D _rigidBody;
    private Knockback _knockback;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        _defaultGravityScale = _rigidBody.gravityScale;
    }

    private void OnEnable()
    {
        _knockback.OnKnockBackStart += KnockbackStart;
        _knockback.OnKnockBackEnd += KnockbackEnd;
        PlayerHealth.OnDeath += HandlePlayerDeath;
        PlayerController.OnJump += ApplyJumpForce;
        PlayerController.OnDash += ApplyDashForce;
    }

    private void OnDisable()
    {
        _knockback.OnKnockBackStart -= KnockbackStart;
        _knockback.OnKnockBackEnd -= KnockbackEnd;
        PlayerHealth.OnDeath -= HandlePlayerDeath;
        PlayerController.OnJump -= ApplyJumpForce;
        PlayerController.OnDash -= ApplyDashForce;
    }

    private void Update()
    {
        GravityDelay();
        if (_dashTime > 0f)
        {
            _dashTime -= Time.deltaTime;
        }
        else
        {
            _dashing = false;
        }
    }

    private void FixedUpdate()
    {
        Move();
        //HandleJump();
        //ExtraGravity();
        JumpAndGravity();
    }

    public void SetCurrentDirection(float currentDirection)
    {
        _moveX = currentDirection;
    }

    private void HandlePlayerDeath(PlayerHealth health)
    {
        _canMove = false;
        _rigidBody.constraints = RigidbodyConstraints2D.None;
        //_rigidBody.linearVelocity = Vector2.zero;
    }

    private void Move()
    {
        if (!_canMove) { return; }

        if (!_dashing)
        {
            Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
            _rigidBody.linearVelocity = movement;
        }
        else
        {
            Debug.Log(_rigidBody.gravityScale);
        }
    }

    private void ApplyJumpForce()
    {
        _timeInAir = 0f;
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    private void ApplyDashForce()
    {
        //_rigidBody.linearVelocity = Vector2.zero;
        Vector2 dashDir = new Vector2(_moveX, 0f).normalized;
        Debug.Log("Dash " + dashDir);
        _rigidBody.AddForce(dashDir * _dashStrength, ForceMode2D.Impulse);
        _dashing = true;
        _dashTime = _dashDuration;
        //_rigidBody.gravityScale = 0f;
        _dashStartPt = transform.position;
        StartCoroutine(DashEnd());
    }

    private IEnumerator DashEnd()
    {
        yield return new WaitForSeconds(_dashDuration);
        _rigidBody.gravityScale = _defaultGravityScale;
        _dashEndPt = transform.position;
    }

    private void KnockbackStart()
    {
        //Debug.Log("Knockback Start");
        _canMove = false;
    }

    private void KnockbackEnd()
    {
        //Debug.Log("Knockback End");
        _canMove = true;
    }

    public void StartTNTKnockback()
    {
        _timeInAir = 0f;
        _inTNTKnockback = true;
        _tntTrailRenderer.emitting = true;
        StartCoroutine(TNTKnockbackEnd());
    }

    private IEnumerator TNTKnockbackEnd()
    {
        yield return new WaitForSeconds(_tntKnockbackDuration);
        _inTNTKnockback = false;
        _timeInAir = 0f;
        _tntTrailRenderer.emitting = false;
    }

    private void GravityDelay()
    {
        if (!PlayerController.Instance.IsGrounded)
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
        if (_timeInAir > _gravityDelay && !_dashing && !_inTNTKnockback)
        {
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime));
            if (_rigidBody.linearVelocityY < _maxFallVelocity)
            {
                _rigidBody.linearVelocityY = _maxFallVelocity;
            }
        }
    }

    private void HandleJump()
    {
        float velY = _rigidBody.linearVelocityY;

        if (velY > 0 && !PlayerController.Instance.IsJumpHeld)
        {
            // Cut jump short when button released
            _rigidBody.AddForce(new Vector2(0f, Physics2D.gravity.y * (_lowJumpMultiplier - 1) * _rigidBody.mass));
        }
        else if (velY < 0)
        {
            // Falling faster
            _rigidBody.AddForce(new Vector2(0f, Physics2D.gravity.y * (_fallMultiplier - 1) * _rigidBody.mass));
        }
    }

    private void JumpAndGravity()
    {
        float velY = _rigidBody.linearVelocityY;

        if (!_dashing && !_inTNTKnockback)
        {
            if (!PlayerController.Instance.IsGrounded && _timeInAir > _gravityDelay)
            {
                _rigidBody.AddForce(Vector2.up * -_extraGravity * Time.fixedDeltaTime);
            }

            if (velY > 0f && !PlayerController.Instance.IsJumpHeld)
            {
                _rigidBody.AddForce(Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * _rigidBody.mass);
            }
            else if (velY < 0f)
            {
                _rigidBody.AddForce(Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * _rigidBody.mass);
            }

            if (_rigidBody.linearVelocityY < _maxFallVelocity)
                _rigidBody.linearVelocityY = _maxFallVelocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.whiteSmoke;
        Gizmos.DrawCube(_dashStartPt, Vector3.one * 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawCube(_dashEndPt, Vector3.one * 0.5f);
    }
}
