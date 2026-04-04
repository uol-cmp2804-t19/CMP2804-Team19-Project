using UnityEngine;
using UnityEngine.Tilemaps;

//TODO add an audio manager to choose sound effect based on terrain and modulate pitch/choose from sound array
// ambient level sound - https://freesound.org/search/?q=ambient+forest
// first level bgm - https://opengameart.org/content/music-for-your-first-level
// research game soundtracks like BabaIsYou

//TODO animate player walk
//TODO make player move to cell gradually rather than snap to - replace moveDelay check with 'is player animating' and queue actions?
//TODO look at separation of concerns - should player move/jump-to cell be a palyer method?

public class LevelController : MonoBehaviour
{
    public PlayerController player = null;
    public Tilemap tilemap = null;
    public float moveDelay = 0.15f;
    private float nextMove = 0.0f;

    // for debugging
    public int player_score = 0;

    public void AddScore()
    {
        player_score++;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Immediately lock player to their current grid cell
    void Start()
    {
        TeleportPlayerToCell(GetPlayerCell());
        //FileIO.WriteFile("test.txt", "new test content");
        //string new_content = FileIO.ReadFile("test.txt")
        //Debug.Log(new_content);
        Debug.Log("new levelcontroller loaded");

        // removed for testing
        // TODO re-enable when config fixed
        //GameManager.Main.Config.LevelsCompleted = 69;
        //GameManager.Main.SaveConfig();
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

        // erase diagonal moves and multiple direction presses, favouring right/up on axis
        Vector2 move_direction = new Vector2(dir.x, dir.y);
        if (x > 0.0)
        {
            move_direction.y = 0.0f;
            player.facing = PlayerController.FACING.RIGHT;
        }
        else if (y > 0.0)
        {
            move_direction.x = 0.0f;
            player.facing = PlayerController.FACING.UP;
        }
        else if (x < 0.0)
        {
            move_direction.y = 0.0f;
            player.facing = PlayerController.FACING.LEFT;
        }
        else if (y < 0.0)
        {
            move_direction.x = 0.0f;
            player.facing = PlayerController.FACING.DOWN;
        }
        else
        {
            // no move
            return;
        }

        MovePlayerInDirection(move_direction);

        // Vector3 movement = new Vector3(x, y, 0);
        // transform.position += movement * speed * Time.deltaTime;

    }

    // draw a red square around the player current cell, for debugging purposes - will appear in origin position until player first move
    void OnDrawGizmos()
    {
        // silently fail, debug handling only
        if (player == null || tilemap == null) return;

        Vector3 center = new Vector3();

        // show world highlight
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

        // show player highlight
        Vector3Int cell = tilemap.WorldToCell(player.transform.position);
        center = tilemap.GetCellCenterWorld(cell);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, tilemap.cellSize);

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
            //TODO - currently player can move diagonally, this will not be possible with coding block calls - does it need to be captured here?
            Vector3Int targetCell = GetPlayerCell() + new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            // temporary teleport/snapping behaviour, to animate gradually eventually
            TeleportPlayerToCell(targetCell);
        }
    }

    void TeleportPlayerToCell(Vector3Int targetCell)
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
        player.PlayWalkSound();

    }

}
