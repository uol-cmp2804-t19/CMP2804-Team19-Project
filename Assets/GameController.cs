using System;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // game objects are declared here so they can be manually added
    public GameObject PlayerCharacter = null;
    private Rigidbody2D PlayerCharacterRB; // declared here for movement
    public GameObject Action1 = null;
    public GameObject Action2 = null;
    public GameObject Action3 = null;

    // currently used for debuging ActionBlocks
    private int actionCounter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the game objects if they are not manually added, debugLogs are used for troubleshooting
        Debug.Log("GameController Started");

        if (PlayerCharacter == null)
        {
            PlayerCharacter = GameObject.Find("PlayerCharacter");
            Debug.Log("'PlayerCharacter' found in Game Controller");
        }
        else Debug.Log("'PlayerCharacter' Manually Added to Game Controller");

        if (PlayerCharacter == null) Debug.LogError("'PlayerCharacter' not found in Game Controller.");

        try
        {
            PlayerCharacterRB = PlayerCharacter.GetComponent<Rigidbody2D>();
            Debug.Log("Player RigidBody Added to Game Controller");
        }
        catch { Debug.LogError("Player RigidBody not found in Game Controller."); }

        if (Action1 == null)
        {
            Action1 = GameObject.Find("ActionBlock1");
            Debug.Log("'ActionBlock1' found in Game Controller.");
        }
        else Debug.Log("'ActionBlock1' Manually Added");

        if (Action1 == null) Debug.LogError("'ActionBlock1' not found in Game Controller.");

        if (Action2 == null)
        {
            Action1 = GameObject.Find("ActionBlock2");
            Debug.Log("'ActionBlock2' found in Game Controller.");
        }
        else Debug.Log("'ActionBlock2' Manually Added");

        if (Action1 == null) Debug.LogError("'ActionBlock2' not found in Game Controller.");

        if (Action3 == null)
        {
            Action1 = GameObject.Find("ActionBlock3");
            Debug.Log("'ActionBlock3' found in Game Controller.");
        }
        else Debug.Log("'ActionBlock3' Manually Added");

        if (Action1 == null) Debug.LogError("'ActionBlock3' not found in Game Controller.");

    }

    /// <summary>
    /// Performs the actions queded in the Action Blocks
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
                MovementDirection = new Vector2(0,1);
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
                Debug.LogError("Something went wrong in CBAction in GameController");
                break;
        }

    }

    public enum CBActionTypes
    {
        up,
        down,
        left,
        right,
        none
    }
}
