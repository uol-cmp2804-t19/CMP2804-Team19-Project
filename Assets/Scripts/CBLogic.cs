using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
// causing build error - temporary removed
//using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using CBClass;
using UnityEngine.Rendering;

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

    List<CodeBlock> ActionBlockObjects = new List<CodeBlock>();

    // assign through bootstrap/gamecontroller on load
    public PlayerController activePlayer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // adds all actionblocks with the correct tag
        // !important! if you want to add action blocks, make sure the have the tag 'ActionBlock'
        List<GameObject> actionBlocks = GameObject.FindGameObjectsWithTag("ActionBlock").ToList<GameObject>();

        // changes tags to empty for later use
        foreach (GameObject block in actionBlocks)
        {
            Debug.Log("ActionBlock '" + block.name + "' added to CBLogic via automated find");
            ActionBlockObjects.Add(new CodeBlock(block, ActionBlockObjects.Count(), false));
            Debug.Log("ActionBlock '" + block.name + "' added as an Object to ActionBlockObjects");
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
        foreach (CodeBlock block in ActionBlockObjects)
        {
            List<String> blockActions = block.GetActions();
            int actionCount = 0;
            foreach (string action in blockActions)
            {
                switch (action)
                {
                    case "Up":
                        actionList.Add(CBActionTypes.up);
                        Debug.Log("Code Block " + block.orderNumber + " contained: Action Up");
                        break;
                    case "Down":
                        actionList.Add(CBActionTypes.down);
                        Debug.Log("Code Block " + block.orderNumber + " contained: Action Down");
                        break;
                    case "Left":
                        actionList.Add(CBActionTypes.left);
                        Debug.Log("Code Block " + block.orderNumber + " contained: Action Left");
                        break;
                    case "Right":
                        actionList.Add(CBActionTypes.right);
                        Debug.Log("Code Block " + block.orderNumber + " contained: Action Right");
                        break;
                    default:
                        Debug.LogError("Code Block " + block.orderNumber + " contains a erroneous action at point: " + actionCount);
                        break;
                }
                actionCount++;
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
    /// <returns> a bool to signal it is completed </returns>
    public bool AddAction(string action)
    {
        foreach (CodeBlock block in ActionBlockObjects)
        {
            if (block.HasSpace)
            {
                block.AssignAction(action);
                return true;
            }
        }
        try
        {
            createBlock(false, action);
            return true; 
        }
        catch { return false; }
    }

    /// <summary>
    /// creates a block
    /// </summary>
    /// <param name="loop"> if it is a loop block assign true</param>
    /// <param name="action"> the action to add </param>
    private void createBlock(bool loop, string action)
    {
        if (loop) 
        { 

        }
        else 
        {

        }

    }
    /// <summary>
    /// Removes an item from the order list
    /// </summary>
    /// <param name="orderNumber"> the order number to be removed </param>
    public void removeActionBlock(int orderNumber)
    {
        
    }

}