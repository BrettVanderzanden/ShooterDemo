using UnityEngine;

public class Basher : MonoBehaviour, IEnemy
{
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private float _knockbackThrust = 5f;
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
        // attack handled in animation
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

            IHittable iHittable = hit.gameObject.GetComponent<IHittable>();
            iHittable?.TakeHit();

            IDamageable iDamageable = hit.gameObject.GetComponent<IDamageable>();
            iDamageable?.TakeDamage(_attackDamage);
            iDamageable?.TakeKnockback(transform.position, _knockbackThrust);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hitboxCenter.position, _damageRadius);
    }
}
