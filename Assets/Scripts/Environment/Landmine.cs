using System;
using System.Collections;
using UnityEngine;

public class Landmine : MonoBehaviour, IHittable
{
    public Action OnLandmineArmed;
    public Action OnLandmineExplode;

    [SerializeField] private int _damageAmount = 100;
    [SerializeField] private float _knockbackThrust = 10f;
    [SerializeField] private float _damageRadius = 2.5f;
    [SerializeField] private float _detonationDelay = 0.1f;
    [SerializeField] private GameObject _explosionVFX;

    private bool _triggered = false;

    private void OnEnable()
    {
        OnLandmineExplode += Explode;
    }

    private void OnDisable()
    {
        OnLandmineExplode -= Explode;
    }

    public void TakeHit()
    {
        // explode instantly from bullets and other explosions
        _detonationDelay = 0f;
        TriggerDetonation();
    }

    private void TriggerDetonation()
    {
        if (!_triggered)
        {
            _triggered = true;
            StartCoroutine(DetonationRoutine());
        }
    }

    private IEnumerator DetonationRoutine()
    {
        yield return new WaitForSeconds(_detonationDelay);
        OnLandmineExplode?.Invoke();
    }

    private void Explode()
    {
        DamageNearby();
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
            iDamageable?.TakeDamage( _damageAmount);
            iDamageable?.TakeKnockback(transform.position, _knockbackThrust);
        }
    }

    private void PlayExplosionVFX()
    {
        Instantiate(_explosionVFX, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>()) // trigger on enemy?
        {
            // play landmine armed sound like a beep
            // landmine armed vfx
            OnLandmineArmed?.Invoke();
            TriggerDetonation();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }

}
