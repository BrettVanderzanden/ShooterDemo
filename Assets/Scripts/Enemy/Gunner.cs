using UnityEngine;
using UnityEngine.Pool;

public class Gunner : MonoBehaviour, IEnemy
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject Gun;

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
        Vector2 targetDir = AimGunAtPlayer();

        Debug.Log("Gunner Shoots");
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, targetDir);
    }

    Vector2 AimGunAtPlayer()
    {
        Vector2 targetDir = PlayerController.Instance.transform.position - Gun.transform.position;

        // Direction from gun to mouse, in parent-local space
        Vector2 direction = (Vector2)(Gun.transform.parent.InverseTransformPoint(targetDir) - transform.localPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Gun.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        return targetDir;
    }
}
