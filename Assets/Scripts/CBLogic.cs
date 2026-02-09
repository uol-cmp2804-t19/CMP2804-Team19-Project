using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CBLogic : MonoBehaviour
{

    // enum to store Coding Block action types
    public enum CBActionTypes
    {
        up,
        down,
        left,
        right,
        none
    }

    // TODO convert action blocks to an ordered list of blocks objects
    public List<GameObject> ActionBlocks = new List<GameObject>();

    public GameObject Action1 = null;
    public GameObject Action2 = null;
    public GameObject Action3 = null;

    // assign through bootstrap/gamecontroller on load
    public PlayerController ActivePlayer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AddActionBlocks(); // commented while still in testing

        if (Action1 == null)
        {
            Action1 = GameObject.Find("ActionBlock1");
            Debug.Log("'ActionBlock1' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock1' Manually Added");

        if (Action1 == null) Debug.LogError("'ActionBlock1' not found in Game Controller - CBLogic.");

        if (Action1.tag == "ActionBlock")
        {
            Action1.tag = "ActionEmpty";
            Debug.Log("'ActionEmpty' added to Action block 1");
        }

        if (Action2 == null)
        {
            Action2 = GameObject.Find("ActionBlock2");
            Debug.Log("'ActionBlock2' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock2' Manually Added");

        if (Action2 == null) Debug.LogError("'ActionBlock2' not found in Game Controller - CBLogic.");

        if (Action2.tag == "ActionBlock")
        {
            Action2.tag = "ActionEmpty";
            Debug.Log("'ActionEmpty' added to Action block 2");
        }

        if (Action3 == null)
        {
            Action3 = GameObject.Find("ActionBlock3");
            Debug.Log("'ActionBlock3' found in Game Controller - CBLogic.");
        }
        else Debug.Log("'ActionBlock3' Manually Added");

        if (Action3 == null) Debug.LogError("'ActionBlock3' not found in Game Controller - CBLogic.");

        if (Action3.tag == "ActionBlock")
        {
            Action3.tag = "ActionEmpty";
            Debug.Log("'ActionEmpty' added to Action block 3");
        }

        ActionBlocks.Add(Action1);
        ActionBlocks.Add(Action2);
        ActionBlocks.Add(Action3);

    }
    // currently seperate from start for testing purposes, will add once guarenteed to work.
    /// <summary>
    /// finds all the action blocks in the scene via the actionBlock Tag, then sets their tag to empty
    /// </summary>
    void AddActionBlocks()
    {
        ActionBlocks = GameObject.FindGameObjectsWithTag("ActionBlock").ToList<GameObject>();
        foreach (GameObject block in ActionBlocks)
        {
            Debug.Log("ActionBlock '" + block.name + "' added to CBLogic");
            block.tag = "ActionEmpty";
            Debug.Log("ActionBlock '" + block.name + "' tag changed to `ActionEmpty`");
        }
    }

    /// <summary>
    /// Adds a Action Block to the Action Blocks List
    /// </summary>
    /// <param name="block">
    /// the action block to be added
    /// </param>
    public void AddActionBlock(GameObject block)
    {
        ActionBlocks.Add(block);
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
        List<CBActionTypes> actions = GetActions();

        if (actions.Count == 0)
        {
            Debug.LogError("'GetActions' in CBLogic did not return a list of actions");
        }
        else
        {
            foreach (CBActionTypes action in actions)
            {
                switch(action)
                {
                    case CBActionTypes.up:
                        Debug.Log("CBLogics performed the 'up' action");
                        CBAction(CBActionTypes.up);
                        break;
                    case CBActionTypes.down:
                        Debug.Log("CBLogics performed the 'down' action");
                        CBAction(CBActionTypes.down);
                        break;
                    case CBActionTypes.left:
                        Debug.Log("CBLogics performed the 'left' action");
                        CBAction(CBActionTypes.left);
                        break;
                    case CBActionTypes.right:
                        Debug.Log("CBLogics performed the 'right' action");
                        CBAction(CBActionTypes.right);
                        break;
                    case CBActionTypes.none:
                        break;
                    default:
                        Debug.LogError("'PerformActions' in CBLogic failed to identify: " + action);
                        break;
                }
            }
        }
    }

    List<CBActionTypes> GetActions()
    {
        List<CBActionTypes> actionList = new List<CBActionTypes>();

        foreach(GameObject action in ActionBlocks)
        {
            switch (action.tag)
            {
                case "ActionEmpty":
                    actionList.Add(CBActionTypes.none);
                    Debug.Log("'"+action.name+"' contained: Action Empty");
                    break;
                case "ActionUp":
                    actionList.Add(CBActionTypes.up);
                    Debug.Log("'" + action.name + "' contained: Action Up");
                    break;
                case "ActionDown":
                    actionList.Add(CBActionTypes.down);
                    Debug.Log("'" + action.name + "' contained: Action Down");
                    break;
                case "ActionLeft":
                    actionList.Add(CBActionTypes.left);
                    Debug.Log("'" + action.name + "' contained: Action Left");
                    break;
                case "ActionRight":
                    actionList.Add(CBActionTypes.right);
                    Debug.Log("'" + action.name + "' contained: Action Right");
                    break;
                default:
                    Debug.LogError("'"+action.name+"' contains a erroneous tag.");
                    break;
            }
        }

        return actionList;
    }


    void CBAction(CBActionTypes CBActionType)
    {
        //TODO determine if to be used, disabled to quiet the compiler warning
        //float movementSpeed = 1f;

        Vector2 MovementDirection = new Vector2(0, 0);
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
    
        // TODO fix
        // move player
        if (ActivePlayer != null)
        {
            ActivePlayer.MovePlayerInDirection(MovementDirection);
}
        else
        {
            Debug.Log("Assign player controller in Game Controller - CBLogic!");
        }

    }

}