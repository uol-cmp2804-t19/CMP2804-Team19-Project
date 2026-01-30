using UnityEngine;

// TODO
// Facing
// Sound when walking (sound manager)
// Ambient level sound (level sound handling, sound manager)
// Grid handling in worldGrid - grid tracked in code when tilemap drawn or physics used? need to snap to grid center if not code validated
// Tilemap square graphics

public class LevelController : MonoBehaviour
{
    public PlayerController player = null;
    public WorldGrid world = null;
    public float moveDelay = 2.5f;
    private float nextMove = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    // Update is called once per frame
    // Update behaviour is just for testing whilst code blocks aren't integrated
    void Update()
    {
        // testing behaviour only
        // TODO remove
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

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

            if (Time.time > nextMove)
            {
                nextMove = Time.time + moveDelay;
                if (world.IsValidMove(direction))
                {
                    player.Move(direction);
                }
            }

        }
    }
}
