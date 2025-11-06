using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _tileMaskPrefab;
    
    void Awake()
    {
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach(Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                BoundsInt bounds = tilemap.cellBounds;

                foreach (Vector3Int pos in bounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(pos))
                    {
                        Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                        Instantiate(_tileMaskPrefab, worldPos, Quaternion.identity, tilemap.transform);
                    }
                }
            }
        }
    }
}
