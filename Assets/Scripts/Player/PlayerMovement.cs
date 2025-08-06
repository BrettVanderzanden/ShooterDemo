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

    private float _moveX;
    private bool _canMove = true;
    private bool _dashing = false;
    private float _dashTime = 0f;
    private float _defaultGravityScale;
    private Vector2 _dashStartPt, _dashEndPt;

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
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    private void ApplyDashForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        Vector2 dashDir = new Vector2(_moveX, 0f).normalized;
        Debug.Log("Dash " + dashDir);
        _rigidBody.AddForce(dashDir * _dashStrength, ForceMode2D.Impulse);
        _dashing = true;
        _dashTime = _dashDuration;
        _rigidBody.gravityScale = 0f;
        _dashStartPt = transform.position;
        StartCoroutine(DashEnd());
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

    private IEnumerator DashEnd()
    {
        yield return new WaitForSeconds(_dashDuration);
        _rigidBody.gravityScale = _defaultGravityScale;
        _dashEndPt = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.whiteSmoke;
        Gizmos.DrawCube(_dashStartPt, Vector3.one * 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawCube(_dashEndPt, Vector3.one * 0.5f);
    }
}
