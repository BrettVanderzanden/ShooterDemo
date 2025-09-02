using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.limeGreen;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
