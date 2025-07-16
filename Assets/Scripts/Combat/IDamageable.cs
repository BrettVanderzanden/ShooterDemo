using UnityEngine;

public interface IDamageable : IHittable
{
    void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockbackThrust);
}
