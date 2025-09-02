using System;
using UnityEngine;

public class TNT : MonoBehaviour
{
    public Action OnTNTExplode;

    [SerializeField] private float _launchForce = 6f;
    [SerializeField] private float _enemyKnockbackThrust = 10f;
    [SerializeField] private float _playerKnockbackThrust = 30f;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _damageRadius = 4f;
    [SerializeField] private float _knockbackRadius = 6f;
    [SerializeField] private float _initialSpinTorque = 3f;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private GameObject _knockbackAreaVFX;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        OnTNTExplode += Explode;
    }

    private void OnDisable()
    {
        OnTNTExplode -= Explode;
    }

    public void Init(Vector2 tntSpawnPos, Vector2 mousePos)
    {
        transform.position = tntSpawnPos;
        Vector2 throwDirection = (mousePos - tntSpawnPos).normalized;
        _rigidBody.AddForce(throwDirection * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_initialSpinTorque);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Stop moving/rotating
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        // attach to enemy?
    }

    public void TriggerDetonation()
    {
        OnTNTExplode?.Invoke();
    }

    private void Explode()
    {
        DamageNearby();
        KnockbackNearby();
        PlayExplosionVFX();
        Destroy(gameObject);
    }

    private void DamageNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _damageRadius);

        foreach (var hit in hits)
        {
            IHittable iHittable = hit.gameObject.GetComponent<IHittable>();
            iHittable?.TakeHit();

            IDamageable iDamageable = hit.gameObject.GetComponent<IDamageable>();
            iDamageable?.TakeDamage(_damageAmount);
        }
    }

    private void KnockbackNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _knockbackRadius);

        foreach (var hit in hits)
        {
            IDamageable iDamageable = hit.gameObject.GetComponent<IDamageable>();

            PlayerController pc = hit.gameObject.GetComponent<PlayerController>();
            if (pc)
            {
                pc.StartTNTKnockback();
            }
            else
            {
                iDamageable?.TakeKnockback(transform.position, _enemyKnockbackThrust);
            }
        }
    }

    private void PlayExplosionVFX()
    {
        Instantiate(_explosionVFX, transform.position, Quaternion.identity);
        Instantiate(_knockbackAreaVFX, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);

        Gizmos.color = Color.softBlue;
        Gizmos.DrawWireSphere(transform.position, _knockbackRadius);
    }

}
