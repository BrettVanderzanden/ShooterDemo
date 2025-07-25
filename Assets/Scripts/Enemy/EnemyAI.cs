using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyAIState CurrentState => _currentState;

    [SerializeField] private MonoBehaviour _enemyType;
    [SerializeField] private float _defaultIdleDuration = 4f;
    [SerializeField] private float _defaultRoamChangeDirTime = 1.5f;
    [SerializeField] private float _idleStateVariationLimit = 2f;
    [SerializeField] private float _minIdleStateDuration = 0.3f;
    [SerializeField] private float _attackCooldown = 1f;

    private float _roamDirection;
    private float _roamTimeLimit;
    private float _timeRoaming = 0f;
    private float _idleTimeLimit;
    private float _timeIdling = 0f;
    private bool _canAttack = true;
    private float _attackRange = 10f;

    private EnemyAIState _currentState;
    private BoxCollider2D _aggroDetection;
    private EnemyPathfinding _enemyPathfinding;
    private EnemyAnimations _animations;

    private void Awake()
    {
        _aggroDetection = GetComponentInChildren<BoxCollider2D>();
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _animations = GetComponent<EnemyAnimations>();

        _attackRange = _aggroDetection.size.x;
    }

    private void Start()
    {
        StartIdle();
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
            switch (_currentState)
            {
                case EnemyAIState.Idle:
                    Idle();
                    break;
                case EnemyAIState.Roaming:
                    Roaming();
                    break;
                case EnemyAIState.Attacking:
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

    private void TransitionTo(EnemyAIState _nextState)
    {
        switch (_nextState)
        {
            case EnemyAIState.Idle:
                StartIdle();
                break;
            case EnemyAIState.Roaming:
                StartRoaming();
                break;
            case EnemyAIState.Attacking:
                StartAttacking();
                break;
            default:
                break;
        }
    }

    private void StartIdle()
    {
        _timeIdling = 0f;
        _timeRoaming = 0f;
        _idleTimeLimit = Mathf.Max(_defaultIdleDuration + Random.Range(-_idleStateVariationLimit, _idleStateVariationLimit), _minIdleStateDuration);
        _currentState = EnemyAIState.Idle;
        _animations.Idle();
    }

    private void Idle()
    {
        _timeIdling += Time.deltaTime;

        _enemyPathfinding.StopMoving();

        if (_timeIdling > _idleTimeLimit)
        {
            StartRoaming();
        }
    }

    private void StartRoaming()
    {
        _currentState = EnemyAIState.Roaming;
        _timeIdling = 0f;
        _timeRoaming = 0f;
        _roamTimeLimit = Mathf.Max(_defaultRoamChangeDirTime + Random.Range(-_idleStateVariationLimit, _idleStateVariationLimit), _minIdleStateDuration);
        _roamDirection = GetRoamingDirection();
        _animations.Roam();
    }

    private void Roaming()
    {
        _timeRoaming += Time.deltaTime;

        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeRoaming > _roamTimeLimit)
        {
            StartIdle();
        }
    }

    private float GetRoamingDirection()
    {
        return Random.Range(-1f, 1f);
    }

    private void StartAttacking()
    {
        _currentState = EnemyAIState.Attacking;
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
            _canAttack = false;
            (_enemyType as IEnemy).Attack();
            _animations.Attack();

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

        if (_timeRoaming > _defaultRoamChangeDirTime)
        {
            StartRoaming();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TransitionTo(EnemyAIState.Attacking);
    }
}

public enum EnemyAIState
{
    Idle,
    Roaming,
    Aggro,
    ReadyingAttack,
    Attacking,
    Hurt,
}
