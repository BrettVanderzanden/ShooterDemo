using System;
using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour, IEnemy
{
    public static Action PistolerShoot;

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;

    private ObjectPool<Bullet> _bulletPool;

    private void Start()
    {
        CreateBulletPool();
    }

    private void OnDestroy()
    {
        _bulletPool.Clear();
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

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    public void Attack()
    {
        Vector2 targetDir = PlayerController.Instance.transform.position - transform.position;
        targetDir.y = 0f;

        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, targetDir);

        PistolerShoot?.Invoke();
    }

    public void StartCombat()
    {
    }

    public void ExitCombat()
    {
    }
}
