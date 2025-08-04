using System;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public Action OnJump;

    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _jumpStrength = 10f;
    [SerializeField] private float _timeTilJump = 0.2f;
    [SerializeField] private float _unmovingDistance = 0.01f;
    [SerializeField] private float _jumpCooldown = 0.8f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;

    private Rigidbody2D _rigidBody;
    private float _facing;
    private float _moveDir;
    private Knockback _knockback;
    private bool _canMove = true;
    private Vector2 _targetPos;
    private float _timeUnmoving = 0f;
    private bool _moving = false;
    private bool _aggroed = false;
    private Vector2 _lastMovedPosition;
    private float _lastJumpTime;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
        _lastMovedPosition = transform.position;
    }

    void OnEnable()
    {
        _knockback.OnKnockBackStart += CanMoveFalse;
        _knockback.OnKnockBackEnd += CanMoveTrue;
        OnJump += ApplyJumpForce;
    }

    void OnDisable()
    {
        _knockback.OnKnockBackStart -= CanMoveFalse;
        _knockback.OnKnockBackEnd -= CanMoveTrue;
        OnJump -= ApplyJumpForce;
    }

    private void Update()
    {
        HandleSpriteFlip();
        Move();
    }

    private void FixedUpdate()
    {
        if (_moving && _canMove)
        {
            float moveDistance = transform.position.x - _lastMovedPosition.x;
            if (Mathf.Abs(moveDistance) > _unmovingDistance)
            {
                _lastMovedPosition = transform.position;
                _timeUnmoving = 0f;
            }
            else
            {
                _timeUnmoving += Time.fixedDeltaTime;
                if (_timeUnmoving >= _timeTilJump && Time.time >= _lastJumpTime && CheckGrounded())
                {
                    Debug.Log("Enemy Jumps");
                    OnJump?.Invoke();
                }
            }
        }
    }

    private void Move()
    {
        if (!_canMove) { return; }

        float moveSpeed = _aggroed ? _runSpeed : _walkSpeed;
        Vector2 movement = new Vector2(_moveDir * moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
    }

    private void ApplyJumpForce()
    {
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        _lastJumpTime = Time.time + _jumpCooldown;
    }

    public void MoveTo(Vector2 movePos)
    {
        _targetPos = movePos;
        _moving = true;
        Vector2 direction = _targetPos - (Vector2)transform.position;
        direction.Normalize();
        _moveDir = Mathf.Sign(direction.x);
        _facing = direction.x;
    }

    // public void MoveToward(float direction)
    // {
    //     _moveDir = direction;
    //     _facing = direction;
    // }

    public void LookToward(float direction)
    {
        _facing = direction;
    }

    private void CanMoveTrue()
    {
        _canMove = true;
    }

    private void CanMoveFalse()
    {
        _canMove = false;
    }

    public void StopMoving()
    {
        _moveDir = 0;
        _targetPos = transform.position;
        _moving = false;
    }

    private void HandleSpriteFlip()
    {
        if (_facing < 0)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    public void SetAggroState(bool aggro)
    {
        _aggroed = aggro;
    }

    public bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
        return isGrounded;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.violetRed;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }
}
