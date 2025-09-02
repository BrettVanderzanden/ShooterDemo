using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private BoxCollider2D _collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyHealth>())
        {
            Debug.Log("Enemy Kill");
            collision.gameObject.GetComponent<EnemyHealth>().Kill();
        }
        else if (collision.gameObject.GetComponent<PlayerHealth>())
        {
            Debug.Log("Player Kill");
            collision.gameObject.GetComponent<PlayerHealth>().Kill();
        }
    }

    private void OnDrawGizmos()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider2D>();
        }
        if (_collider == null) return;
        Bounds bounds = _collider.bounds;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

    }
}
