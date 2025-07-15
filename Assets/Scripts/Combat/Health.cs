using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    public static Action<Health> OnDeath;

    [SerializeField] private int _startingHealth = 100;

    private Health _health;
    private int _currentHealth;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(Vector2 damageSourceDir, int damageAmount)
    {
        _health.TakeDamage(damageAmount);
    }

    public void TakeHit()
    {
        // hit fx
    }

}
