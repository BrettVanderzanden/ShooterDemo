using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyAIState CurrentState => _currentState;

    [SerializeField] private MonoBehaviour _enemyType;
    [SerializeField] private float _defaultIdleDuration = 4f;
    [SerializeField] private float _idleStateVariationLimit = 2f;
    [SerializeField] private float _minIdleStateDuration = 0.3f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _aggroRange = 10f;
    [SerializeField] private float _aggroPauseDuration = 1f;
    [SerializeField] private float _aggroCooldown = 4f; // aggro should probably be reworked...
    [SerializeField] private float _minRoamingDistance = 4f;
    [SerializeField] private float _maxRoamingDistance = 10f;
    [SerializeField] private float _roamCloseEnough = 0.2f;
    [SerializeField] private float _roamTimeLimit = 6f;

    private float _timeInCurrentState = 0f;
    private Vector2 _roamTarget;
    private float _idleTimeLimit;
    private bool _canAttack = true;
    private float _lastAggroTime;

    private EnemyAIState _currentState;
    private EnemyPathfinding _enemyPathfinding;
    private EnemyAnimations _animations;

    private void Awake()
    {
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _animations = GetComponent<EnemyAnimations>();
    }

    private void Start()
    {
        TransitionTo(EnemyAIState.Idle);
        SetRoamingTarget();
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
        _enemyPathfinding.SetAggroState(false);
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
        SetRoamingTarget();
        _animations.Walk();
        _enemyPathfinding.SetAggroState(false);
    }

    private void Roaming()
    {
        _enemyPathfinding.MoveTo(_roamTarget);

        float targetDistance = transform.position.x - _roamTarget.x;

        if (Mathf.Abs(targetDistance) < _roamCloseEnough || _timeInCurrentState >= _roamTimeLimit)
        {
            TransitionTo(EnemyAIState.Idle);
        }
    }

    private void SetRoamingTarget()
    {
        float roamingDistance = Random.Range(-_maxRoamingDistance, _maxRoamingDistance);
        if (Mathf.Sign(roamingDistance) > 0)
        {
            roamingDistance = Mathf.Max(_minRoamingDistance, roamingDistance);
        }
        else
        {
            roamingDistance = Mathf.Min(-_minRoamingDistance, roamingDistance);
        }
        _roamTarget = new Vector2(transform.position.x + roamingDistance, transform.position.y);
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
        _enemyPathfinding.SetAggroState(true);
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
        else // (Vector3.Distance(transform.position, targetPos) > _attackRange)
        {
            _enemyPathfinding.MoveTo(targetPos);
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
        _enemyPathfinding.SetAggroState(false);
        _enemyPathfinding.MoveTo(_roamTarget);

        float targetDistance = transform.position.x - _roamTarget.x;

        if (Mathf.Abs(targetDistance) < _roamCloseEnough || _timeInCurrentState >= _roamTimeLimit)
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
        if (_currentState == EnemyAIState.Roaming)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_roamTarget, Vector3.one);
        }
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
