using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 10f;

    private float _moveX;
    private bool _canMove = true;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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

    private void Move()
    {
        if (!_canMove) { return; }

        Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
    }
}
