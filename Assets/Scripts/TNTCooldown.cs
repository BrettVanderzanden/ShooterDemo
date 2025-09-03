using UnityEngine;

public class TNTCooldown : MonoBehaviour
{
    [SerializeField] public float _TNTCooldownTime = 1f;

    private float _timer = 1f;
    public float _fillFraction;

    private void Update()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        _timer += Time.deltaTime;
        if (_timer >= _TNTCooldownTime)
        {
            _timer = _TNTCooldownTime;
        }
        _fillFraction = _timer / _TNTCooldownTime;
    }

    public void StartCooldown()
    {
        _timer = 0f;
    }
}
