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
    public WorldMap world = null;
    public Tilemap tilemap = null;
    public float moveDelay = 0.15f;
    private float nextMove = 0.0f;

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

        MovePlayer(dir);

        // Vector3 movement = new Vector3(x, y, 0);
        // transform.position += movement * speed * Time.deltaTime;

    }

    void MovePlayer(Vector2 direction)
    {
        if (player == null || world == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or grid!");
            return;
        }
        else
        {
            // check for blockages - placeholder does nothing currently, passes dir intentionally rn
            if (!world.IsValidMove(direction)) return;

            //Tilemap tilemap = world.GetComponent<Tilemap>();
            Vector3Int currentCell = tilemap.WorldToCell(player.transform.position);
            Vector3Int targetCell = currentCell + new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            player.transform.position = tilemap.GetCellCenterWorld(targetCell);

        }
    }
}
