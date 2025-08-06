using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Health>())
        {
            Debug.Log("Enemy Kill");
            collision.gameObject.GetComponent<Health>().Kill();
        }
        else if (collision.gameObject.GetComponent<PlayerHealth>())
        {
            Debug.Log("Player Kill");
            collision.gameObject.GetComponent<PlayerHealth>().Kill();
        }
    }
}
