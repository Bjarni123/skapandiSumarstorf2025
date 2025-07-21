using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeManager : MonoBehaviour 
{
    public Tilemap trunkTilemap;
    public Tilemap canopyTilemap;
    
    public void CutTree(Vector3Int position)
    {
        // Remove canopy
        canopyTilemap.SetTile(position, null);
        
        // Remove collider at this position
        trunkTilemap.GetComponent<TilemapCollider2D>().enabled = false;
        // Re-enable after updating (Unity will rebuild colliders)
        trunkTilemap.GetComponent<TilemapCollider2D>().enabled = true;
    }
}