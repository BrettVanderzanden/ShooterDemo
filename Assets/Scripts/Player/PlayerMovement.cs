using UnityEngine;
using System;

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
    private float _dashingTime = 0f;

    private Rigidbody2D _rigidBody;
    private Knockback _knockback;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    private void OnEnable()
    {
        _knockback.OnKnockBackStart += CanMoveFalse;
        _knockback.OnKnockBackEnd += CanMoveTrue;
        PlayerHealth.OnDeath += HandlePlayerDeath;
        PlayerController.OnJump += ApplyJumpForce;
        PlayerController.OnDash += ApplyDashForce;
    }

    private void OnDisable()
    {
        _knockback.OnKnockBackStart -= CanMoveFalse;
        _knockback.OnKnockBackEnd -= CanMoveTrue;
        PlayerHealth.OnDeath -= HandlePlayerDeath;
        PlayerController.OnJump -= ApplyJumpForce;
        PlayerController.OnDash -= ApplyDashForce;
    }

    private void Update()
    {
        if (_dashingTime > 0f)
        {
            _dashingTime -= Time.deltaTime;
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

    private void CanMoveTrue()
    {
        _canMove = true;
    }

    private void CanMoveFalse()
    {
        _canMove = false;
    }

    private void HandlePlayerDeath(PlayerHealth health)
    {
        _canMove = false;
        _rigidBody.linearVelocity = Vector2.zero;
    }

    private void Move()
    {
        if (!_canMove) { return; }

        if (!_dashing)
        {

        Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
        }
    }

    private void ApplyJumpForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    private void ApplyDashForce()
    {
        Vector2 dashDir = new Vector2(_rigidBody.linearVelocityX, 0f).normalized;
        Debug.Log(dashDir);
        _rigidBody.AddForce(dashDir * _dashStrength, ForceMode2D.Impulse);
        _dashing = true;
        _dashingTime = _dashDuration;
    }
}
