using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public enum FACING { UP, DOWN, LEFT, RIGHT };
    public FACING facing = FACING.DOWN;
    public AudioSource sound_walk = null;
    private float speed = 400.0f;
    public Tilemap tilemap = null;

    // Update is called once per frame
    // placeholder testing behaviour
    //void Update()
    //{
    //    float x = Input.GetAxisRaw("Horizontal");
    //    float y = Input.GetAxisRaw("Vertical");

    //    Vector3 movement = new Vector3(x, y, 0);
    //    transform.position += movement * speed * Time.deltaTime;
    //}

    //TODO remove/tidy
    // debug testing movement for player input
    public void Move(Vector2 dir)
    {
        Vector3 movement = new Vector3(dir.x, dir.y, 0);
        transform.position += movement * speed * Time.deltaTime;
    }

    public void PlayWalkSound()
    {
        if (sound_walk != null)
        {
            sound_walk.Play();
        }
    }

    public Vector3Int GetPlayerCell()
    {
        if (tilemap == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return new Vector3Int(0, 0, 0);
        }
        else
        {
            Vector3Int currentCell = tilemap.WorldToCell(transform.position);
            return currentCell;
        }
    }

    public void MovePlayerInDirection(Vector2 direction)
    {
        if (tilemap == null)
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

    public void TeleportPlayerToCell(Vector3Int targetCell)
    {
        if (tilemap == null)
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
        transform.position =
        tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position)
        );
        //player.transform.position = tilemap.GetCellCenterWorld(targetCell);
        // move player to center of tile
        transform.position = tilemap.CellToWorld(targetCell) + tilemap.cellSize / 2f;
        Debug.Log("on map");
        PlayWalkSound();

    }
}
