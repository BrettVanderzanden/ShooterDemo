using System;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public Action OnDynamiteExplode;
    
    [SerializeField] private float _launchForce = 6f;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _damageRadius = 4f;
    [SerializeField] private float _initialSpinTorque = 3f;

    private Rigidbody2D _rigidBody;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void Init(Vector2 tntSpawnPos, Vector2 mousePos)
    {
        transform.position = tntSpawnPos;
        Vector2 throwDirection = (mousePos - tntSpawnPos).normalized;
        _rigidBody.AddForce(throwDirection * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_initialSpinTorque);
    }

}
