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
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _aggroRange = 10f;
    [SerializeField] private float _aggroPauseDuration = 1f;
    [SerializeField] private float _aggroCooldown = 4f; // aggro should probably be reworked...

    private float _timeInCurrentState = 0f;
    private float _roamDirection;
    private float _roamTimeLimit;
    private float _idleTimeLimit;
    private bool _canAttack = true;
    private float _lastAggroTime;

    private EnemyAIState _currentState;
    //private BoxCollider2D _aggroDetection;
    private EnemyPathfinding _enemyPathfinding;
    private EnemyAnimations _animations;

    private void Awake()
    {
        //_aggroDetection = GetComponentInChildren<BoxCollider2D>();
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _animations = GetComponent<EnemyAnimations>();

        //_attackRange = _aggroDetection.size.x;
    }

    private void Start()
    {
        TransitionTo(EnemyAIState.Idle);
        GetRoamingDirection();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        _timeInCurrentState += Time.deltaTime;
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
                case EnemyAIState.Aggro:
                    Aggroing();
                    break;
                case EnemyAIState.MovingToAttack:
                    MovingToAttack();
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
        _timeInCurrentState = 0f;
        switch (_nextState)
        {
            case EnemyAIState.Idle:
                StartIdle();
                break;
            case EnemyAIState.Roaming:
                StartRoaming();
                break;
            case EnemyAIState.Aggro:
                StartAggro();
                break;
            case EnemyAIState.MovingToAttack:
                StartMovingToAttack();
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
        //Debug.Log("StartIdle");
        _idleTimeLimit = Mathf.Max(_defaultIdleDuration + Random.Range(-_idleStateVariationLimit, _idleStateVariationLimit), _minIdleStateDuration);
        _currentState = EnemyAIState.Idle;
        _animations.Idle();
        _enemyPathfinding.StopMoving();
    }

    private void Idle()
    {
        if (_timeInCurrentState > _idleTimeLimit)
        {
            TransitionTo(EnemyAIState.Roaming);
        }
    }

    private void StartRoaming()
    {
        //Debug.Log("StartRoaming");
        _currentState = EnemyAIState.Roaming;
        _roamTimeLimit = Mathf.Max(_defaultRoamChangeDirTime + Random.Range(-_idleStateVariationLimit, _idleStateVariationLimit), _minIdleStateDuration);
        _roamDirection = GetRoamingDirection();
        _animations.Walk();
    }

    private void Roaming()
    {
        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeInCurrentState > _roamTimeLimit)
        {
            TransitionTo(EnemyAIState.Idle);
        }
    }

    private float GetRoamingDirection()
    {
        return Random.Range(-1f, 1f);
    }

    private void StartAggro()
    {
        //Debug.Log("StartAggro");
        _lastAggroTime = Time.time;
        _currentState = EnemyAIState.Aggro;
        _enemyPathfinding.StopMoving();
        Vector2 targetDir = PlayerController.Instance.transform.position - transform.position;
        _enemyPathfinding.LookToward(targetDir.x);
        // play aggro animation
        _animations.Idle();
    }

    private void Aggroing()
    {
        if (_timeInCurrentState >= _aggroPauseDuration)
        {
            TransitionTo(EnemyAIState.MovingToAttack);
        }
    }

    private void StartMovingToAttack()
    {
        //Debug.Log("StartMovingToAttack");
        _currentState = EnemyAIState.MovingToAttack;
        _animations.Walk();
    }

    private void MovingToAttack()
    {
        Vector3 targetPos = PlayerController.Instance.transform.position;
        if (Vector3.Distance(transform.position, targetPos) <= _attackRange)
        {
            TransitionTo(EnemyAIState.Attacking);
            return;
        }

        if (Vector3.Distance(transform.position, targetPos) > _aggroRange)
        {
            TransitionTo(EnemyAIState.Idle);
        }
        else // if (Vector3.Distance(transform.position, targetPos) > _attackRange)
        {
            Vector2 targetDir = targetPos - transform.position;
            _enemyPathfinding.MoveToward(targetDir.x);
        }
    }

    private void StartAttacking()
    {
        //Debug.Log("StartAttacking");
        _currentState = EnemyAIState.Attacking;
        _enemyPathfinding.StopMoving();
    }

    private void Attacking()
    {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > _attackRange)
        {
            TransitionTo(EnemyAIState.MovingToAttack);
        }

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
        _enemyPathfinding.MoveToward(_roamDirection);

        if (_timeInCurrentState > _defaultRoamChangeDirTime)
        {
            TransitionTo(EnemyAIState.Roaming);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _lastAggroTime > _aggroCooldown)
        {
            TransitionTo(EnemyAIState.Aggro);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPt = transform.position;
        Vector3 endPt = transform.position;
        endPt.x += _attackRange;
        Gizmos.DrawLine(startPt, endPt);
        Gizmos.color = Color.yellow;
        endPt.x = transform.position.x + _aggroRange;
        endPt.y += .2f;
        startPt.y = endPt.y;
        Gizmos.DrawLine(startPt, endPt);
    }
}

public enum EnemyAIState
{
    Idle,
    Roaming,
    Aggro,
    MovingToAttack,
    Attacking,
    Hurt,
}
