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

    [Header("Dynamite")]
    [SerializeField] private Dynamite _dynamitePrefab;

    private ObjectPool<Bullet> _bulletPool;

    private Vector2 _mousePos;
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private float _lastFireTime = 0f;

    private Animator _animator;
    private PlayerInput _playerInput;
    private FrameInput _frameInput;

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
    }

    private void OnDisable()
    {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnTNTThrow -= ThrowTNT;
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
    }

    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos);
    }

    private void ThrowTNT()
    {
        Dynamite newTNT = Instantiate(_dynamitePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        newTNT.Init(_bulletSpawnPoint.position, _mousePos);
    }

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }

    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
