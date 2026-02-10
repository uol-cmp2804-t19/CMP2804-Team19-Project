using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle;
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
    public List<GameObject> actionBlocks = new List<GameObject>();

    // game objects are declared here for manual assignment
    public GameObject Action1 = null;
    public GameObject Action2 = null;
    public GameObject Action3 = null;

    // assign through bootstrap/gamecontroller on load
    public PlayerController activePlayer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // adds all actionblocks with the correct tag
        // !important! if you want to add action blocks, make sure the have the tag 'ActionBlock'
        actionBlocks = GameObject.FindGameObjectsWithTag("ActionBlock").ToList<GameObject>();

        // changes tags to empty for later use
        foreach (GameObject block in actionBlocks)
        {
            Debug.Log("ActionBlock '" + block.name + "' added to CBLogic via automated find");
            block.tag = "ActionEmpty";
            Debug.Log("ActionBlock '" + block.name + "' tag changed to `ActionEmpty`");
        }

        actionBlocks = actionBlocks.OrderBy(ab => ab.name).ToList(); // sorts the action blocks via name so it reads correctly

        // adds the player if not manually assigned, !important! player should have the tag 'Player'
        if (activePlayer == null)
        {
            activePlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            Debug.Log("Player successfully added");
        }
        else { Debug.Log("Active Player Manually Assigned to CBLogic."); }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gets the actions to be performed, converts them to a in code output and sends to CBAction
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
                switch (action)
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
    /// <summary>
    /// reads action blocks and returns a list of actions
    /// </summary>
    /// <returns>
    /// the list of actions to be performed
    /// </returns>
    List<CBActionTypes> GetActions()
    {
        List<CBActionTypes> actionList = new List<CBActionTypes>();

        foreach (GameObject action in actionBlocks)
        {
            switch (action.tag)
            {
                case "ActionEmpty":
                    actionList.Add(CBActionTypes.none);
                    Debug.Log("'" + action.name + "' contained: Action Empty");
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
                    Debug.LogError("'" + action.name + "' contains a erroneous tag.");
                    break;
            }
        }

        return actionList;
    }

    /// <summary>
    /// performs the selected action
    /// </summary>
    /// <param name="CBActionType">
    /// the action to be performed
    /// </param>
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

        // move player
        if (activePlayer != null)
        {
            activePlayer.MovePlayerInDirection(MovementDirection);
        }
        else
        {
            Debug.Log("Assign player controller in Game Controller - CBLogic!");
        }

    }

    /// <summary>
    /// adds an action to the last empty Action Block
    /// </summary>
    public void AddAction(string action)
    {
        string tag = null;
        switch (action)
        {
            case "up":
                tag = "ActionUp";
                break;
            case "down":
                tag = "ActionUp";
                break;
            case "left":
                tag = "ActionUp";
                break;
            case "right":
                tag = "ActionUp";
                break;
            default:
                Debug.Log("");
                break;
        }
        if (tag != null)
        {
            foreach (GameObject block in actionBlocks)
            {
                if (block.tag == "ActionEmpty") { block.tag = tag; break; }
            }
        }
    }

}