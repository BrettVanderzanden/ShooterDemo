using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>, IDamageable
{
    public static Action<PlayerHealth> OnDeath;
    public bool IsDead { get; private set; }

    [SerializeField] private int _startingHealth = 100;
    [SerializeField] private float _damageRecoveryTime = 1f;
    [SerializeField] private float _deathLoadSceneWaitTime = 2f;

    private Knockback _knockback;
    private Slider _healthSlider;
    private int _currentHealth;
    private bool _canTakeDamage = true;
    const string HEALTH_SLIDER_NAME = "Health Bar";

    protected override void Awake()
    {
        base.Awake();

        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        IsDead = false;
        _currentHealth = _startingHealth;
        UpdateHealthSlider();
    }

    public void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockbackThrust)
    {
        if (!_canTakeDamage) { return; }

        _canTakeDamage = false;
        _currentHealth -= damageAmount;

        UpdateHealthSlider();

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

    private void UpdateHealthSlider()
    {
        if (_healthSlider == null)
        {
            _healthSlider = GameObject.Find(HEALTH_SLIDER_NAME).GetComponent<Slider>(); // string references are bad
        }
        _healthSlider.maxValue = _startingHealth;
        _healthSlider.value = _currentHealth;
    }
}
