using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public bool IsValidMove(Vector2 direction)
    {
        //public Tilemap map = null;
        // discard direction no use
        if (direction == Vector2.zero)
        {
            return false;
        }
        else
        {
            //TODO need to be able to reference what is an occupied cell in the tilemap
            Debug.Log("ADD FUNCTIONALITY FOR CHECKING IF GRID IS BLOCKED");
            return true;
        }
    }
}
