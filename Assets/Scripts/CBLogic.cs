using UnityEngine;

public class CBLogic : MonoBehaviour
{
    public GameObject Action1 = null;
    public GameObject Action2 = null;
    public GameObject Action3 = null;

    // currently used for debuging ActionBlocks
    private int actionCounter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Action1 == null)
        {
            Action1 = GameObject.Find("ActionBlock1");
            Debug.Log("'ActionBlock1' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock1' Manually Added");

        if (Action1 == null) Debug.LogError("'ActionBlock1' not found in Game Controller - CBLogic.");

        if (Action2 == null)
        {
            Action2 = GameObject.Find("ActionBlock2");
            Debug.Log("'ActionBlock2' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock2' Manually Added");

        if (Action2 == null) Debug.LogError("'ActionBlock2' not found in Game Controller - CBLogic.");

        if (Action3 == null)
        {
            Action3 = GameObject.Find("ActionBlock3");
            Debug.Log("'ActionBlock3' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock3' Manually Added");

        if (Action3 == null) Debug.LogError("'ActionBlock3' not found in Game Controller - CBLogic.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Performs the actions queued in the Action Blocks
    /// </summary>
    public void PerformActions()
    {
        // currently just moves the character around to debug movement
        Debug.Log("'PerformActions' called");


        CBAction(CBActionTypes.up);
    }

    void CBAction(CBActionTypes CBActionType)
    {
        float movementSpeed = 1f;
        Vector2 MovementDirection;
        switch (CBActionType)
        {
            case CBActionTypes.up:
                MovementDirection = new Vector2(0, 1);
                break;
            case CBActionTypes.down:
                MovementDirection = new Vector2(0, -1);
                break;
            case CBActionTypes.left:
                MovementDirection = new Vector2(-1, 0);
                break;
            case CBActionTypes.right:
                MovementDirection = new Vector2(1, 0);
                break;
            default:
                Debug.LogError("Something went wrong in CBAction in Game Controller - CBLogic.");
                break;
        }

    }
    // enum to store Coding Block action types
    public enum CBActionTypes
    {
        up,
        down,
        left,
        right,
        none // ideally, this should never be used
    }
}
