using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public static Action OnShoot;
    public static Action OnTNTThrow;

    [Header("Bullet")]
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = 0.5f;

    [Header("TNT")]
    [SerializeField] private TNT _tntPrefab;
    [SerializeField] private float _tntThrowCD = 2f;

    private ObjectPool<Bullet> _bulletPool;

    private Vector3 _mousePos;
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private static readonly int THROW_TNT_HASH = Animator.StringToHash("Throw TNT");
    private float _lastFireTime = 0f;
    private bool _tntIsActive = false;
    private float _lastTNTTime = 0f;

    private Animator _animator;
    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private TNT _activeTNT;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInput;
    }

    void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        GatherInput();
        Shoot();
        RotateGun();
    }

    private void OnEnable()
    {
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnTNTThrow += ThrowTNT;
        OnTNTThrow += ThrowTNTAnimation;
    }

    private void OnDisable()
    {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnTNTThrow -= ThrowTNT;
        OnTNTThrow -= ThrowTNTAnimation;
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet);
        }, false, 20, 40);
    }

    private void Shoot()
    {
        if (_frameInput.Shoot && Time.time >= _lastFireTime)
        {
            OnShoot?.Invoke();
        }

        if (_frameInput.TNT)
        {
            if (!_tntIsActive)
            {
                if (Time.time >= _lastTNTTime)
                {
                    _tntIsActive = true;
                    OnTNTThrow?.Invoke();
                }
            }
            else
            {
                _tntIsActive = false;
                _activeTNT.TriggerDetonation();
                _lastTNTTime = Time.time + _tntThrowCD;
            }
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos);
    }

    private void ThrowTNT()
    {
        TNT newTNT = Instantiate(_tntPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        newTNT.Init(_bulletSpawnPoint.position, _mousePos);
        _activeTNT = newTNT;
    }

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    private void ThrowTNTAnimation()
    {
        _animator.Play(THROW_TNT_HASH, 0, 0f);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }

    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 direction = _mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.limeGreen;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
