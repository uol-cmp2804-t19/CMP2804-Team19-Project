using UnityEngine;
using UnityEngine.Tilemaps;

// TODO
// Facing
// Sound when walking (sound manager)
// Ambient level sound (level sound handling, sound manager)
// Grid handling in WorldMap - grid tracked in code when tilemap drawn or physics used? need to snap to grid center if not code validated
// Tilemap square graphics

public class LevelController : MonoBehaviour
{
    public PlayerController player = null;
    public Tilemap tilemap = null;
    public float moveDelay = 0.15f;
    private float nextMove = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Immediately lock player to their current grid cell
    void Start()
    {
        MovePlayerToCell(GetPlayerCell());
    }

    // Update is called once per frame
    // Update behaviour is just for testing whilst code blocks aren't integrated
    // TODO remove this
    void Update()
    {
        // testing behaviour only
        // TODO remove
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        // is it a valid input/is a move currently allowed
        if (dir == Vector2.zero) return;
        if (Time.time <= nextMove) return;
        nextMove = Time.time + moveDelay;

        if (x > 0.0)
        {
            Debug.Log("Move Right");
        }
        else if (y > 0.0)
        {
            Debug.Log("Move Up");
        }
        else if (x < 0.0)
        {
            Debug.Log("Move Left");
        }
        else if (y < 0.0)
        {
            Debug.Log("Move Down");
        }

        MovePlayerInDirection(dir);

        // Vector3 movement = new Vector3(x, y, 0);
        // transform.position += movement * speed * Time.deltaTime;

    }

    // draw a red square around the player current cell, for debugging purposes - will appear in origin position until player first move
    void OnDrawGizmos()
    {
        // silently fail, debug handling only
        if (player == null || tilemap == null) return;

        Vector3Int cell = GetPlayerCell();
        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, tilemap.cellSize);

        //TODO this is an unnecessary draw call every drawGizmos call just do it once?
        TileBase[] allTiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        // iterate over the x axis
        for (int x = 0; x < tilemap.cellBounds.size.x; x++)
        {
            // iterate over the y axis
            for (int y = 0; y < tilemap.cellBounds.size.y; y++)
            {
                // flattened array of all tiles indexed by x+y*width
                TileBase tile = allTiles[x + y * tilemap.cellBounds.size.x];
                if (tile != null)
                {
                    Vector3Int cellPos = new Vector3Int(x + tilemap.cellBounds.x, y + tilemap.cellBounds.y, 0);
                    center = tilemap.GetCellCenterWorld(cellPos);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawWireCube(center, tilemap.cellSize);
                }
            }
        }


        //
    }

    Vector3Int GetPlayerCell()
    {
        if (player == null || tilemap == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return new Vector3Int(0, 0, 0);
        }
        else
        {
            Vector3Int currentCell = tilemap.WorldToCell(player.transform.position);
            return currentCell;
        }
    }

    void MovePlayerInDirection(Vector2 direction)
    {
        if (player == null || tilemap == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        else
        {
            Vector3Int targetCell = GetPlayerCell() + new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            MovePlayerToCell(targetCell);
        }
    }

    void MovePlayerToCell(Vector3Int targetCell)
    {
        if (player == null || tilemap == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }

        if (!tilemap.HasTile(targetCell))
        {
            Debug.Log("off map");
            return;
        }

        // snap to grid
        player.transform.position =
        tilemap.GetCellCenterWorld(tilemap.WorldToCell(player.transform.position)
        );
        //player.transform.position = tilemap.GetCellCenterWorld(targetCell);
        // move player to center of tile
        player.transform.position = tilemap.CellToWorld(targetCell) + tilemap.cellSize / 2f;
        Debug.Log("on map");
    
    }

}
