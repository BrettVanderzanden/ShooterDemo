using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 10f;

    private float _moveX;
    private bool _canMove = true;

    private Rigidbody2D _rigidBody;
    private Knockback _knockback;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    void OnEnable()
    {
        _knockback.OnKnockBackStart += CanMoveFalse;
        _knockback.OnKnockBackEnd += CanMoveTrue;
        PlayerHealth.OnDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        _knockback.OnKnockBackStart -= CanMoveFalse;
        _knockback.OnKnockBackEnd -= CanMoveTrue;
        PlayerHealth.OnDeath -= HandlePlayerDeath;
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

        Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
    }
}
