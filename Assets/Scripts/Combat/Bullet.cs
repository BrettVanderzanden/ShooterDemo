using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Action OnBulletHitFlesh;
    public static Action OnBulletHitSolid;

    [SerializeField] private GameObject _bulletVFX;
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _maxLifeTime = 2f;
    [SerializeField] private float _knockbackThrust = 30;
    [SerializeField] private float _knockbackTime = .2f;

    private Vector2 _fireDirection;
    private Rigidbody2D _rigidBody;
    private Gun _gun;
    private Shooter _shooter;
    private Gunner _gunner;
    private float _expirationTime;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidBody.linearVelocity = _fireDirection * _moveSpeed;
        if (Time.fixedTime >= _expirationTime)
        {
            ReleaseBullet();
        }
    }

    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mousePos)
    {
        _gun = gun;
        transform.position = bulletSpawnPos;
        _fireDirection = (mousePos - bulletSpawnPos).normalized;
        _expirationTime = Time.time + _maxLifeTime;
    }

    public void Init(Shooter shooter, Vector2 bulletSpawnPos, Vector2 targetDir)
    {
        _shooter = shooter;
        transform.position = bulletSpawnPos;
        _fireDirection = targetDir.normalized;
        _expirationTime = Time.time + _maxLifeTime;
    }

    public void Init(Gunner gunner, Vector2 bulletSpawnPos, Vector2 targetDir)
    {
        _gunner = gunner;
        transform.position = bulletSpawnPos;
        _fireDirection = targetDir.normalized;
        _expirationTime = Time.time + _maxLifeTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(_bulletVFX, transform.position, Quaternion.identity);

        IHittable iHittable = other.gameObject.GetComponent<IHittable>();
        iHittable?.TakeHit();

        IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(_damageAmount);
        iDamageable?.TakeKnockback(transform.position, _knockbackThrust, _knockbackTime);

        if (iDamageable != null)
        {
            OnBulletHitFlesh?.Invoke();
        }
        else
        {
            OnBulletHitSolid?.Invoke();
        }
        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        if (_gun != null)
        {
            _gun.ReleaseBulletFromPool(this);
        }
        else if (_shooter != null)
        {
            _shooter.ReleaseBulletFromPool(this);
        }
        else if (_gunner != null)
        {
            _gunner.ReleaseBulletFromPool(this);
        }
    }
}