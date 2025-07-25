using System.Collections;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private float _attackDuration = 1f;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        _animator.SetBool("isIdle", true);
        _animator.SetBool("isMoving", false);
        _animator.SetBool("isAttacking", false);
    }

    public void Roam()
    {
        _animator.SetBool("isIdle", false);
        _animator.SetBool("isMoving", true);
        _animator.SetBool("isAttacking", false);
    }

    public void Attack()
    {
        _animator.SetBool("isIdle", false);
        _animator.SetBool("isMoving", false);
        _animator.SetBool("isAttacking", true);
    }
}
