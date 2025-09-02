using UnityEngine;
using System;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public Action OnKnockBackStart;
    public Action OnKnockBackEnd;

    private Vector3 _hitDirection;
    private float _knockbackThrust;
    private float _knockbackTime;

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

    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust, float knockbackTime)
    {
        _hitDirection = hitDirection;
        _knockbackThrust = knockbackThrust;
        _knockbackTime = knockbackTime;

        OnKnockBackStart?.Invoke();
    }

    private void ApplyKnockbackForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        Vector2 direction = transform.position - _hitDirection;
        direction.y = 0; // left or right only
        Vector3 difference = (direction).normalized * _knockbackThrust * _rigidBody.mass;
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
        //_rigidBody.linearVelocity = Vector2.zero;
    }
}
