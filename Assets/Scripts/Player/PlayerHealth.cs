using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>, IDamageable
{
    public static Action<PlayerHealth> OnDeath;
    public bool IsDead { get; private set; }

    [SerializeField] private int _startingHealth = 100;
    [SerializeField] private float _damageRecoveryTime = 1f;
    [SerializeField] private float _deathLoadSceneWaitTime = 2f;

    private Knockback _knockback;
    private int _currentHealth;
    private bool _canTakeDamage = true;

    protected override void Awake()
    {
        base.Awake();

        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        IsDead = false;
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockbackThrust)
    {
        if (!_canTakeDamage) { return; }

        _canTakeDamage = false;
        _currentHealth -= damageAmount;
        
        CheckAlive();

        if (!IsDead)
        {
            _knockback.GetKnockedBack(damageSourceDir, knockbackThrust);
            // dmg recovery fx here
            GetComponentInChildren<SpriteRenderer>().color = Color.blue;
            StartCoroutine(DamageRecoveryRoutine());
        }
    }

    public void TakeHit()
    {
        // hit fx
    }

    private void CheckAlive()
    {
        if (_currentHealth <= 0 && !IsDead)
        {
            _currentHealth = 0;
            IsDead = true;
            // death animation here
            OnDeath?.Invoke(this);
            GetComponent<BoxCollider2D>().enabled = false;
        
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    private IEnumerator DeathLoadSceneRoutine()
    {
        yield return new WaitForSeconds(_deathLoadSceneWaitTime);
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = !IsDead;
        // stop dmg recovery fx here
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}
