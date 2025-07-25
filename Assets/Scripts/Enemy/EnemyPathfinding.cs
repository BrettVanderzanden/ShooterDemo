using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rigidBody;
    private float _facing;
    private float _moveDir;
    private Knockback _knockback;
    private bool _canMove = true;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    void OnEnable()
    {
        _knockback.OnKnockBackStart += CanMoveFalse;
        _knockback.OnKnockBackEnd += CanMoveTrue;
    }

    void OnDisable()
    {
        _knockback.OnKnockBackStart -= CanMoveFalse;
        _knockback.OnKnockBackEnd -= CanMoveTrue;
    }

    private void Update()
    {
        HandleSpriteFlip();
        Move();
    }

    private void Move()
    {
        if (!_canMove) { return; }

        Vector2 movement = new Vector2(_moveDir * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
    }

    public void MoveToward(float direction)
    {
        _moveDir = direction;
        _facing = direction;
    }

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
}
