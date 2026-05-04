using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public GameObject sprite_down = null;
    public GameObject sprite_up = null;
    public GameObject sprite_left = null;
    public GameObject sprite_right = null;
    public Vector3Int startCell;

    public enum FACING
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public FACING facing = FACING.DOWN;
    public AudioSource sound_walk = null;

    // unused for now but will be used to control player movement speed and animation eventually
    // private float speed = 400.0f;
    public LevelMapManager level = null;
    public int zLevel = 0;

    // currently used for debug movement input but could be extended for 'check next move allowed' or queueing actions
    public float moveDelay = 0.15f;
    private float nextMove = 0.0f;
    public bool useDebugMoveWASD = true;

    private void Start()
    {
        turnPlayer(FACING.DOWN);
        startCell = GetPlayerCell();
    }

    void Update()
    {
        if (useDebugMoveWASD == true)
        {
            CheckMovementInput();
        }
    }

    public Vector3Int GetPlayerCell()
    {
        if (level == null)
        {
            Debug.Log("You forgot to assign a player and/or map!");
            return new Vector3Int(0, 0, 0);
        }
        else
        {
            Vector3Int currentCell = level.GetPlayerCell();
            return currentCell;
        }
    }

    /// <summary>
    /// jumps the player in the direction they are currently facing using previous movement code
    /// This increments z level then moves - used for jump blocks
    /// </summary>
    public void JumpPlayerByFacing()
    {
        if (level == null)
        {
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        
        // player zLevel only updated if jump is successful or during jump
        MovePlayerByFacing(isJump: true);
    }

    /// <summary>
    /// moves the player in the direction they are currently facing using previous movement code
    /// </summary>
    public void MovePlayerByFacing(bool isJump = false)
    {
        if (level == null)
        {
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        Vector2 dir = Vector2.zero;
        switch (facing)
        {
            case FACING.UP:
                dir = Vector2.up;
                break;
            case FACING.DOWN:
                dir = Vector2.down;
                break;
            case FACING.LEFT:
                dir = Vector2.left;
                break;
            case FACING.RIGHT:
                dir = Vector2.right;
                break;
        }
        int z_change = isJump ? 1 : 0;
        level.MovePlayerOnGrid(new Vector3Int((int)dir.x, (int)dir.y, (int)z_change));
    }

    public void PlayWalkSound()
    {
        if (sound_walk != null)
        {
            sound_walk.Play();
        }
    }

    /// <summary>
    /// turns the player right or left depending on the argument (right is x = +1 so true, left is x =-1 so false)
    /// </summary>
    /// <param name="turnRight">
    /// true to turn right, false to turn left
    /// </param>
    public void TurnPlayer(bool turnRight)
    {
        if (turnRight)
        {
            switch (facing)
            {
                case FACING.UP:
                    turnPlayer(FACING.RIGHT);
                    break;

                case FACING.RIGHT:
                    turnPlayer(FACING.DOWN);
                    break;

                case FACING.DOWN:
                    turnPlayer(FACING.LEFT);
                    break;

                case FACING.LEFT:
                    turnPlayer(FACING.UP);
                    break;
            }
        }
        else
        {
            // TURNING LEFT
            switch (facing)
            {
                case FACING.UP:
                    turnPlayer(FACING.LEFT);
                    break;

                case FACING.LEFT:
                    turnPlayer(FACING.DOWN);
                    break;

                case FACING.DOWN:
                    turnPlayer(FACING.RIGHT);
                    break;

                case FACING.RIGHT:
                    turnPlayer(FACING.UP);
                    break;
            }
        }
    }

    // changes internal facing tracker & updates graphic
    // clumsy approach but future proofing for animation setup rather than changing image in code
    private void turnPlayer(FACING new_direction)
    {
        // do graphic
        facing = new_direction;

        // single line as bulks code out otherwise
        if (sprite_up == null) { Debug.Log("You have not set player sprite_up in inspector!"); return; }
        if (sprite_down == null) { Debug.Log("You have not set player sprite_down in inspector!"); return; }
        if (sprite_left == null) { Debug.Log("You have not set player sprite_left in inspector!"); return; }
        if (sprite_right == null) { Debug.Log("You have not set player sprite_right in inspector!"); return; }

        sprite_up.GetComponent<SpriteRenderer>().enabled = false;
        sprite_down.GetComponent<SpriteRenderer>().enabled = false;
        sprite_left.GetComponent<SpriteRenderer>().enabled = false;
        sprite_right.GetComponent<SpriteRenderer>().enabled = false;
        sprite_up.SetActive(false);
        sprite_down.SetActive(false);
        sprite_left.SetActive(false);
        sprite_right.SetActive(false);

        switch (new_direction)
        {
            case FACING.UP:
                sprite_up.SetActive(true);
                sprite_up.GetComponent<SpriteRenderer>().enabled = true;
                return;

            case FACING.DOWN:
                sprite_down.SetActive(true);
                sprite_down.GetComponent<SpriteRenderer>().enabled = true;
                return;

            case FACING.LEFT:
                sprite_left.SetActive(true);
                sprite_left.GetComponent<SpriteRenderer>().enabled = true;
                return;

            case FACING.RIGHT:
                sprite_right.SetActive(true);
                sprite_right.GetComponent<SpriteRenderer>().enabled = true;
                return;

        }
    }


    private void CheckMovementInput()
    {
        if (level == null)
        {
            return;
        }

        // testing behaviour only
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
            Debug.Log("YOU ARE GOING RIGHT");
            turnPlayer(FACING.RIGHT);
        }
        else if (y > 0.0)
        {
            move_direction.x = 0.0f;
            turnPlayer(FACING.UP);
        }
        else if (x < 0.0)
        {
            Debug.Log("YOU ARE GOING LEFT");
            move_direction.y = 0.0f;
            turnPlayer(FACING.LEFT);
        }
        else if (y < 0.0)
        {
            move_direction.x = 0.0f;
            turnPlayer(FACING.DOWN);
        }
        else
        {
            // no move
            return;
        }

        level.MovePlayerOnGrid(new Vector3Int((int)move_direction.x, (int)move_direction.y, zLevel));

        // Vector3 movement = new Vector3(x, y, 0);
        // transform.position += movement * speed * Time.deltaTime;

    }
}
