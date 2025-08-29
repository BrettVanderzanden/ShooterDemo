using UnityEngine;
using UnityEngine.Pool;

public class Gunner : MonoBehaviour, IEnemy
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _Gun;
    [SerializeField] private float _accuracyDeviation = 5f;

    private ObjectPool<Bullet> _bulletPool;
    private Vector2 _targetDir;
    private bool _inCombat = false;

    private void Start()
    {
        CreateBulletPool();
    }

    private void OnDestroy()
    {
        _bulletPool.Clear();
    }

    private void Update()
    {
        if (_inCombat)
        {
            // we want to draw this out of debug probably
            RaycastHit2D raycastHitPlayer = Physics2D.Raycast(_bulletSpawnPoint.position, _targetDir, 10f, LayerMask.NameToLayer("Player"));
            Debug.DrawRay(_bulletSpawnPoint.position, _targetDir, Color.red);
        }
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

    public void StartCombat()
    {
        AimGunAtPlayer();
        _inCombat = true;
    }

    public void Attack()
    {
        AimGunAtPlayer();

        // Debug.Log("Gunner Shoots");
        Vector2 bulletDirection = GetShootDirection();

        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, bulletDirection);

        Debug.DrawRay(_bulletSpawnPoint.position, bulletDirection, Color.green, 0.2f);
    }

    public void ExitCombat()
    {
        _inCombat = false;
    }

    private void AimGunAtPlayer()
    {
        Vector2 playerCenter = PlayerController.Instance.GetCenterPosition();
        Vector2 targetDir = playerCenter - (Vector2)_Gun.transform.position;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        if (transform.eulerAngles.y == 180f)
        {
            angle = 180f - angle;
        }
        _Gun.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        _targetDir = targetDir;
    }

    private Vector2 GetShootDirection()
    {
        float spread = Random.Range(-_accuracyDeviation, _accuracyDeviation);
        Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spread);
        return spreadRotation * _targetDir;//.normalized;

    }

    private void OnDrawGizmos()
    {
        if (_inCombat)
        {
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, _accuracyDeviation);
            Debug.DrawRay(_bulletSpawnPoint.position, spreadRotation * _targetDir, Color.orange);
            spreadRotation = Quaternion.Euler(0f, 0f, -_accuracyDeviation);
            Debug.DrawRay(_bulletSpawnPoint.position, spreadRotation * _targetDir, Color.orange);
        }
    }
}
