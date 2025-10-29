using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static Action<EnemyHealth> OnTakeDamage;
    public static Action<EnemyHealth> OnDeath;

    [SerializeField] private int _startingHealth = 100;

    private Knockback _knockback;
    private EnemyAI _enemyAI;
    private int _currentHealth;

    private void Awake()
    {
        _knockback = GetComponent<Knockback>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        _enemyAI.TakeDamage();

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void Kill()
    {
        TakeDamage(_currentHealth);
    }

    public void TakeKnockback(Vector2 damageSourceDir, float knockbackThrust, float knockbackTime)
    {
        _knockback.GetKnockedBack(damageSourceDir, knockbackThrust, knockbackTime);
    }

    public void TakeHit()
    {
        // hit fx
    }

}
