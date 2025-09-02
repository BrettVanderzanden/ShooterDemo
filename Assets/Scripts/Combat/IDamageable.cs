using UnityEngine;

public interface IDamageable : IHittable
{
    void TakeDamage(int _damageAmount);
    void TakeKnockback(Vector2 damageSourceDir, float knockbackThrust, float knockbackTime);
    void Kill();
}
