using System;
using UnityEngine;

public class Basher : MonoBehaviour, IEnemy
{
    public static Action BasherAttack;
    public static Action BasherHit;

    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private float _knockbackThrust = 5f;
    [SerializeField] private float _knockbackTime = .3f;
    [SerializeField] private float _damageRadius = 3f;
    [SerializeField] private Transform _hitboxCenter;

    private int _playerLayer;
    private Collider2D[] _attackHits;

    private void Awake()
    {
        //_hitboxCenter = transform.Find("Hitbox");
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    public void Attack()
    {
        BasherAttack?.Invoke();
    }

    public void StartCombat()
    {
    }

    public void ExitCombat()
    {
    }

    public void DealDamage()
    {
        Debug.Log("Basher Deal Damage");
        _attackHits = Physics2D.OverlapCircleAll(_hitboxCenter.position, _damageRadius);
        foreach (var hit in _attackHits)
        {
            Debug.Log("Basher Deal Damage hit");

            BasherHit?.Invoke();

            IHittable iHittable = hit.gameObject.GetComponent<IHittable>();
            iHittable?.TakeHit();

            IDamageable iDamageable = hit.gameObject.GetComponent<IDamageable>();
            iDamageable?.TakeDamage(_attackDamage);
            iDamageable?.TakeKnockback(transform.position, _knockbackThrust, _knockbackTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hitboxCenter.position, _damageRadius);
    }
}
