using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _jumpInterval = 4f;
    [SerializeField] private float _changeDirectionInterval = 3f;

    private Movement _movement;
    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
    }

    private void Start()
    {
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(RandomJumpRoutine());
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            int _currentDirection = UnityEngine.Random.Range(0, 2) * 2 - 1; // 1 or -1
            _movement.SetCurrentDirection(_currentDirection);
            yield return new WaitForSeconds(_changeDirectionInterval);
        }
    }

    private IEnumerator RandomJumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);
            float randomDirection = Random.Range(-1, 1);
            Vector2 jumpDirection = new Vector2(randomDirection, 1f).normalized;
            _rigidBody.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (!player) { return; }

        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement.CanMove)
        {
            // IHittable iHittable = collision.gameObject.GetComponent<IHittable>();
            // iHittable?.TakeHit();

            // IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            // iDamageable?.TakeDamage(transform.position, _damageAmount, _knockbackThrust);

            // AudioManager.Instance.Enemy_OnPlayerHit();
        }
    }
}
