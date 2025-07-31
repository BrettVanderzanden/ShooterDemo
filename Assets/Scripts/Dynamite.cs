using System;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public Action OnDynamiteExplode;

    [SerializeField] private float _launchForce = 6f;
    [SerializeField] private float _knockbackThrust = 10f;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _damageRadius = 4f;
    [SerializeField] private float _initialSpinTorque = 3f;
    [SerializeField] private GameObject _explosionVFX;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        OnDynamiteExplode += Explode;
    }

    private void OnDisable()
    {
        OnDynamiteExplode -= Explode;
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
    }

    public void TriggerDetonation()
    {
        OnDynamiteExplode?.Invoke();
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
            iDamageable?.TakeDamage(transform.position, _damageAmount, _knockbackThrust);
        }
    }

    private void PlayExplosionVFX()
    {
        Instantiate(_explosionVFX, transform.position, Quaternion.identity);
    }

}
