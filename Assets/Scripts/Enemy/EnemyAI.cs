using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        Roaming,
        Attacking
    }

    [SerializeField] private MonoBehaviour _enemyType;
    [SerializeField] private float _idleTimer = 4f;
    [SerializeField] private float _roamChangeDirTimer = 1.5f;
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private float _attackCooldown = 1f;

    private float _roamDirection;
    private float _timeRoaming = 0f;
    private float _timeIdling = 0f;
    private bool _canAttack = true;

    private State _state;
    private EnemyPathfinding _enemyPathfinding;

    private void Awake()
    {
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _state = State.Roaming;
    }

    private void Start()
    {
        GetRoamingDirection();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        switch (_state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Roaming:
                Roaming();
                break;
            case State.Attacking:
                Attacking();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        _timeIdling += Time.deltaTime;

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < _attackRange)
        {
            _state = State.Attacking;
        }

        _enemyPathfinding.StopMoving();

        if (_timeIdling > _idleTimer)
        {
            _state = State.Roaming;
            _roamDirection = GetRoamingDirection();
            Debug.Log("Roaming");
        }
    }

    private void Roaming()
    {
        _timeRoaming += Time.deltaTime;

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < _attackRange)
        {
            _state = State.Attacking;
        }

        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeRoaming > _roamChangeDirTimer)
        {
            _state = State.Idle;
            Debug.Log("Idle");
        }
    }

    private float GetRoamingDirection()
    {
        _timeIdling = 0f;
        _timeRoaming = 0f;
        return Random.Range(-1f, 1f);
    }

    private void Attacking()
    {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > _attackRange)
        {
            _state = State.Roaming;
            Debug.Log("Roaming");
            _roamDirection = GetRoamingDirection();
        }

        _enemyPathfinding.StopMoving();
        Vector2 targetDir = PlayerController.Instance.transform.position - transform.position;
        _enemyPathfinding.LookToward(targetDir.x);

        if (_attackRange != 0 && _canAttack)
        {
            Debug.Log("Attacking");
            _canAttack = false;
            (_enemyType as IEnemy).Attack();

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
