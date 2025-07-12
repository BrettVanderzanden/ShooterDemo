using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rigidBody;
    private float _facing;
    private float _moveDir;
    private Vector2 _movePosition;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleSpriteFlip();
    }

    private void FixedUpdate()
    {
        Vector2 movePosition = _rigidBody.position;
        movePosition.x += _moveDir * (_moveSpeed * Time.fixedDeltaTime);
        _rigidBody.MovePosition(movePosition);
    }

    public void MoveTo(Vector2 targetPosition)
    {
        _movePosition = targetPosition;
        _facing = targetPosition.x; // fix
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

    public void StopMoving()
    {
        _moveDir = 0;
        _movePosition = transform.position;
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
