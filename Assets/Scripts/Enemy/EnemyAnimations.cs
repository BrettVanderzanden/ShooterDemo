using System.Collections;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    private static readonly int IDLE_HASH = Animator.StringToHash("Idle");
    private static readonly int WALK_HASH = Animator.StringToHash("Walk");
    private static readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        _animator.Play(IDLE_HASH, 0, 0f);
    }

    public void Walk()
    {
        _animator.Play(WALK_HASH, 0, 0f);
    }

    public void Attack()
    {
        _animator.Play(ATTACK_HASH, 0, 0f);
    }
}
