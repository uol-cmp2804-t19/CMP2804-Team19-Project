using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
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

    public Vector3Int GetPlayerCell()
    {
        if (level == null)
        {
            //TODO add error handling
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
    /// moves the player in the direction they are currently facing using previous movement code
    /// </summary>
    public void MovePlayerByFacing()
    {
        if (level == null)
        {
            //TODO add error handling
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
        level.MovePlayerInDirection(dir);
    }

    //TODO move to playerAudioController?
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
                case FACING.UP:    facing = FACING.RIGHT; break;
                case FACING.RIGHT: facing = FACING.DOWN;  break;
                case FACING.DOWN:  facing = FACING.LEFT;  break;
                case FACING.LEFT:  facing = FACING.UP;    break;
            }
        }
        else
        {
            switch (facing)
            {
                case FACING.UP:    facing = FACING.LEFT;  break;
                case FACING.LEFT:  facing = FACING.DOWN;  break;
                case FACING.DOWN:  facing = FACING.RIGHT; break;
                case FACING.RIGHT: facing = FACING.UP;    break;
            }
        }
    }

}
