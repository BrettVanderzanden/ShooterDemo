using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private float _attackHeight = 10f;
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
        if (PlayerController.Instance)
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
        else
        {
            MindlessRoaming();
        }
        
    }

    private void StartIdle()
    {
        //Debug.Log("Idle");
        _timeIdling = 0f;
        _timeRoaming = 0f;
        _state = State.Idle;
    }

    private void Idle()
    {
        _timeIdling += Time.deltaTime;

        if (CheckPlayerInAttackArea())
        {
            _state = State.Attacking;
        }

        _enemyPathfinding.StopMoving();

        if (_timeIdling > _idleTimer)
        {
            StartRoaming();
        }
    }

    private void StartRoaming()
    {
        //Debug.Log("Roaming");
        _state = State.Roaming;
        _timeIdling = 0f;
        _timeRoaming = 0f;
        _roamDirection = GetRoamingDirection();
    }

    private void Roaming()
    {
        _timeRoaming += Time.deltaTime;

        if (CheckPlayerInAttackArea())
        {
            _state = State.Attacking;
        }

        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeRoaming > _roamChangeDirTimer)
        {
            StartIdle();
        }
    }

    private float GetRoamingDirection()
    {
        return Random.Range(-1f, 1f);
    }

    private bool CheckPlayerInAttackArea()
    {
        Rect attackRangeBox = new Rect(transform.position.x - _attackRange,
                                        transform.position.y - _attackHeight,
                                        _attackRange*2,
                                        _attackHeight*2);
        return attackRangeBox.Contains(PlayerController.Instance.transform.position);
    }

    private void Attacking()
    {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > _attackRange)
        {
            StartRoaming();
        }

        _enemyPathfinding.StopMoving();
        Vector2 targetDir = PlayerController.Instance.transform.position - transform.position;
        _enemyPathfinding.LookToward(targetDir.x);

        if (_attackRange != 0 && _canAttack)
        {
            //Debug.Log("Attacking");
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

    private void MindlessRoaming()
    {
        _timeRoaming += Time.deltaTime;

        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeRoaming > _roamChangeDirTimer)
        {
            StartRoaming();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, _attackRange);
        Vector3 attackRangeA = new Vector3(transform.position.x - _attackRange, transform.position.y);
        Vector3 attackRangeB = new Vector3(transform.position.x + _attackRange, transform.position.y);
        Gizmos.DrawLine(attackRangeA, attackRangeB);
        Vector3 attackHeightA = new Vector3(transform.position.x, transform.position.y - _attackHeight);
        Vector3 attackHeightB = new Vector3(transform.position.x, transform.position.y + _attackHeight);
        Gizmos.DrawLine(attackHeightA, attackHeightB);
    }
}
