using UnityEngine;
using UnityEngine.Tilemaps;

// logic for movement moved to LevelManager

/// <summary>
/// LevelLayer is a component for each tilemap layer, containing references to the player and tilemap and drawing debug gizmos
/// </summary>
public class LevelLayer : MonoBehaviour
{
    public Tilemap tilemap = null;
    public int zLevel = 0;

    public void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("You forgot to assign a tilemap to the level layer at " + zLevel + "(aka " + gameObject.name + ")!");
        }
    }

    // disabled for release build
    // draw a red square around the player current cell, for debugging purposes - will appear in origin position until player first move
    // void OnDrawGizmos()
    // {
    //     if (tilemap != null)
    //     {
    //         Vector3 center = new Vector3();

    //         // show world highlight
    //         //TODO this is an unnecessary draw call every drawGizmos call just do it once?
    //         TileBase[] allTiles = tilemap.GetTilesBlock(tilemap.cellBounds);
    //         // iterate over the x axis
    //         for (int x = 0; x < tilemap.cellBounds.size.x; x++)
    //         {
    //             // iterate over the y axis
    //             for (int y = 0; y < tilemap.cellBounds.size.y; y++)
    //             {
    //                 // flattened array of all tiles indexed by x+y*width
    //                 TileBase tile = allTiles[x + y * tilemap.cellBounds.size.x];
    //                 if (tile != null)
    //                 {
    //                     Vector3Int cellPos = new Vector3Int(x + tilemap.cellBounds.x, y + tilemap.cellBounds.y, 0);
    //                     center = tilemap.GetCellCenterWorld(cellPos);

    //                     Gizmos.color = Color.grey;
    //                     Gizmos.DrawWireCube(center, tilemap.cellSize);
    //                 }
    //             }
    //         }
    //     }
    // }

}
