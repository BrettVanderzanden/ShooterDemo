using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public static Action<Health> OnDeath;

    [SerializeField] private int _startingHealth = 100;

    private Knockback _knockback;
    private int _currentHealth;

    private void Awake()
    {
        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
    }

    private void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockbackThrust)
    {
        TakeDamage(damageAmount);
        _knockback.GetKnockedBack(damageSourceDir, knockbackThrust);
    }

    public void TakeHit()
    {
        // hit fx
    }

}
