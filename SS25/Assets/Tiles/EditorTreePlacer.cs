using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TreeData
{
    [Header("Tree Tiles")]
    public TileBase[] trunkTiles;
    public TileBase[] canopyTiles;
    
    [Header("Trunk Layout")]
    public int trunkWidth = 1;
    public int trunkHeight = 1;
    public Vector3Int trunkOffset = Vector3Int.zero;
    
    [Header("Canopy Layout")]
    public int canopyWidth = 3;
    public int canopyHeight = 4;
    public Vector3Int canopyOffset = new Vector3Int(-1, 1, 0);
}

public class EditorTreePlacer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap trunkTilemap;
    public Tilemap canopyTilemap;
    public Tilemap collisionTilemap;
    
    [Header("Tree Types")]
    public TreeData[] treeTypes;
    public int selectedTreeType = 0;
    
    [Header("Collision Settings")]
    public TileBase collisionTile; // Single tile for collision areas
    public int collisionWidth = 3;
    public int collisionHeight = 2;
    public Vector3Int collisionOffset = new Vector3Int(-1, 0, 0);
    
    [Header("Tool Settings")]
    public bool enablePlacement = true;
    public bool showPreview = true;
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!enablePlacement || !showPreview) return;
        if (treeTypes == null || selectedTreeType >= treeTypes.Length) return;
        
        // Show preview at mouse position
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        mousePos.z = 0;
        
        if (trunkTilemap != null)
        {
            Vector3Int cellPos = trunkTilemap.WorldToCell(mousePos);
            Vector3 worldPos = trunkTilemap.CellToWorld(cellPos);
            
            // Draw preview gizmo
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(worldPos + Vector3.one * 0.5f, Vector3.one);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(EditorTreePlacer))]
public class EditorTreePlacerEditor : Editor
{
    EditorTreePlacer placer;
    
    void OnEnable()
    {
        placer = (EditorTreePlacer)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        GUILayout.Label("Tree Placement Controls", EditorStyles.boldLabel);
        
        if (placer.treeTypes != null && placer.treeTypes.Length > 0)
        {
            string[] treeNames = new string[placer.treeTypes.Length];
            for (int i = 0; i < treeNames.Length; i++)
            {
                treeNames[i] = "Tree Type " + i;
            }
            
            placer.selectedTreeType = EditorGUILayout.Popup("Selected Tree Type", placer.selectedTreeType, treeNames);
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Auto-Find Tilemaps"))
        {
            placer.trunkTilemap = GameObject.Find("TreeTrunks")?.GetComponent<Tilemap>();
            placer.canopyTilemap = GameObject.Find("TreeCanopies")?.GetComponent<Tilemap>();
            placer.collisionTilemap = GameObject.Find("TreeCollision")?.GetComponent<Tilemap>();
            
            if (placer.trunkTilemap != null && placer.canopyTilemap != null)
            {
                Debug.Log("Found visual tilemaps successfully!");
            }
            else
            {
                Debug.LogWarning("Could not find TreeTrunks and/or TreeCanopies tilemaps.");
            }
            
            if (placer.collisionTilemap == null)
            {
                Debug.LogWarning("Could not find TreeCollision tilemap. Create one if you want collision.");
            }
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Hold Ctrl + Left Click in Scene View to place trees\nHold Shift + Left Click to remove trees\n\nMake sure to create a 'TreeCollision' tilemap for collision!", MessageType.Info);
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(placer);
        }
    }
    
    void OnSceneGUI()
    {
        if (!placer.enablePlacement) return;
        if (placer.treeTypes == null || placer.selectedTreeType >= placer.treeTypes.Length) return;
        
        Event e = Event.current;
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            mousePosition.z = 0;
            
            if (placer.trunkTilemap != null)
            {
                Vector3Int cellPosition = placer.trunkTilemap.WorldToCell(mousePosition);
                
                if (e.control)
                {
                    PlaceTree(cellPosition);
                    e.Use();
                }
                else if (e.shift)
                {
                    RemoveTree(cellPosition);
                    e.Use();
                }
            }
        }
    }
    
    void PlaceTree(Vector3Int position)
    {
        if (placer.selectedTreeType >= placer.treeTypes.Length) return;
        
        TreeData tree = placer.treeTypes[placer.selectedTreeType];
        
        // Place trunk
        if (placer.trunkTilemap != null && tree.trunkTiles != null)
        {
            int tileIndex = 0;
            for (int y = 0; y < tree.trunkHeight; y++)
            {
                for (int x = 0; x < tree.trunkWidth; x++)
                {
                    if (tileIndex < tree.trunkTiles.Length && tree.trunkTiles[tileIndex] != null)
                    {
                        Vector3Int trunkPos = position + tree.trunkOffset + new Vector3Int(x, y, 0);
                        placer.trunkTilemap.SetTile(trunkPos, tree.trunkTiles[tileIndex]);
                    }
                    tileIndex++;
                }
            }
        }
        
        // Place canopy
        if (placer.canopyTilemap != null && tree.canopyTiles != null)
        {
            int tileIndex = 0;
            for (int y = 0; y < tree.canopyHeight; y++)
            {
                for (int x = 0; x < tree.canopyWidth; x++)
                {
                    if (tileIndex < tree.canopyTiles.Length && tree.canopyTiles[tileIndex] != null)
                    {
                        Vector3Int canopyPos = position + tree.canopyOffset + new Vector3Int(x, y, 0);
                        placer.canopyTilemap.SetTile(canopyPos, tree.canopyTiles[tileIndex]);
                    }
                    tileIndex++;
                }
            }
        }
        
        // Place collision area (3x2 grid)
        if (placer.collisionTilemap != null && placer.collisionTile != null)
        {
            for (int y = 0; y < placer.collisionHeight; y++)
            {
                for (int x = 0; x < placer.collisionWidth; x++)
                {
                    Vector3Int collisionPos = position + placer.collisionOffset + new Vector3Int(x, y, 0);
                    placer.collisionTilemap.SetTile(collisionPos, placer.collisionTile);
                }
            }
        }
        
        EditorUtility.SetDirty(placer.trunkTilemap);
        EditorUtility.SetDirty(placer.canopyTilemap);
        if (placer.collisionTilemap != null)
            EditorUtility.SetDirty(placer.collisionTilemap);
    }
    
    void RemoveTree(Vector3Int position)
    {
        if (placer.selectedTreeType >= placer.treeTypes.Length) return;
        
        TreeData tree = placer.treeTypes[placer.selectedTreeType];
        
        // Remove trunk
        if (placer.trunkTilemap != null)
        {
            for (int y = 0; y < tree.trunkHeight; y++)
            {
                for (int x = 0; x < tree.trunkWidth; x++)
                {
                    Vector3Int trunkPos = position + tree.trunkOffset + new Vector3Int(x, y, 0);
                    placer.trunkTilemap.SetTile(trunkPos, null);
                }
            }
        }
        
        // Remove canopy
        if (placer.canopyTilemap != null)
        {
            for (int y = 0; y < tree.canopyHeight; y++)
            {
                for (int x = 0; x < tree.canopyWidth; x++)
                {
                    Vector3Int canopyPos = position + tree.canopyOffset + new Vector3Int(x, y, 0);
                    placer.canopyTilemap.SetTile(canopyPos, null);
                }
            }
        }
        
        // Remove collision area
        if (placer.collisionTilemap != null)
        {
            for (int y = 0; y < placer.collisionHeight; y++)
            {
                for (int x = 0; x < placer.collisionWidth; x++)
                {
                    Vector3Int collisionPos = position + placer.collisionOffset + new Vector3Int(x, y, 0);
                    placer.collisionTilemap.SetTile(collisionPos, null);
                }
            }
        }
        
        EditorUtility.SetDirty(placer.trunkTilemap);
        EditorUtility.SetDirty(placer.canopyTilemap);
        if (placer.collisionTilemap != null)
            EditorUtility.SetDirty(placer.collisionTilemap);
    }
}
#endif