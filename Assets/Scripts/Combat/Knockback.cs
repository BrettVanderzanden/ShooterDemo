using UnityEngine;
using System;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public Action OnKnockBackStart;
    public Action OnKnockBackEnd;

    [SerializeField] private float _knockbackTime = 0.25f;

    private Vector3 _hitDirection;
    private float _knockbackThrust;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        OnKnockBackStart += ApplyKnockbackForce;
        OnKnockBackEnd += StopKnockbackRoutine;
    }

    private void OnDisable()
    {
        OnKnockBackStart -= ApplyKnockbackForce;
        OnKnockBackEnd -= StopKnockbackRoutine;
    }

    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust)
    {
        _hitDirection = hitDirection;
        _knockbackThrust = knockbackThrust;

        OnKnockBackStart?.Invoke();
    }

    private void ApplyKnockbackForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        Vector3 difference = (transform.position - _hitDirection).normalized * _knockbackThrust * _rigidBody.mass;
        difference.y = 0;
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(_knockbackTime);
        OnKnockBackEnd?.Invoke();
    }

    private void StopKnockbackRoutine()
    {
        _rigidBody.linearVelocity = Vector2.zero;
    }
}
