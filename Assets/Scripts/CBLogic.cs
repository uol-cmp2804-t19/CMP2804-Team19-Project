using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// causing build error - temporary removed
//using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using CBClass;
using UnityEngine.Rendering;

/// <summary>
/// Behaviour to read the action blocks and perform the actions on the player
/// Requires the player to exist when this is added - player is added to levels, if this isn't a child of level the startup needs recalled
/// PerformActions() -> GetActions() -> CBAction() -> PlayerController functions
/// </summary>
public class CBLogic : MonoBehaviour
{

    /// <summary>
    /// enum to store Coding Block action types
    /// Used in getActions to convert the string output of the code blocks to a in code output for CBAction
    /// Used in CBAction to determine what action to perform
    /// When adding a new action it needs to be specified here.
    /// </summary>
    //Move to CBClass? Add a getString/printer method there? Why are we using strings?
    public enum CBActionTypes
    {
        MOVE,
        TURNLEFT,
        TURNRIGHT,
        JUMP,
        NONE
    }

    List<CodeBlock> ActionBlockObjects = new List<CodeBlock>();

    // assign through bootstrap/gamecontroller on load
    public PlayerController activePlayer = null;
    // Time control for block queue execution
    [SerializeField] private float actionDelay = 0.5f;
    private bool isExecutingQueue = false;

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
            Debug.Log("ActionBlock '" + block.name + "' added as an Object to ActionBlockObjects");
        }
    
        //should this be actionBlockObjects?
        actionBlocks = actionBlocks.OrderBy(ab => ab.name).ToList(); // sorts the action blocks via name so it reads correctly

        // adds the player if not manually assigned, !important! player should have the tag 'Player'
        // This only works if the player exists - but player is a child of level so if level is reloaded this needs resetting
        if (activePlayer == null)
        {
            activePlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            Debug.Log("Player successfully added");
        }
        else { Debug.Log("Active Player Manually Assigned to CBLogic."); }

    }

    // Update is called once per frame
    /*
    void Update()
    {

    }
    */

    /// <summary>
    /// Gets the actions to be performed, converts them to a in code output and sends to CBAction
    /// When adding a new action needs to be updated here.
    /// </summary>
    public void PerformActions(List<CBActionTypes> actions)
    {
        if (isExecutingQueue)
        {
            Debug.LogWarning("CBLogic: Action queue is already running.");
            return;
        }

        Debug.Log("CBLogic: PerformActions called");

        if (actions.Count == 0)
        {
            Debug.LogWarning("CBLogic: No actions found in queue.");
            return;
        }

        StartCoroutine(ExecuteActionsWithDelay(actions));
    }

    private IEnumerator ExecuteActionsWithDelay(List<CBActionTypes> actions)
    {
        isExecutingQueue = true;

        foreach (CBActionTypes action in actions)
        {
            switch (action)
            {
                case CBActionTypes.MOVE:
                    Debug.Log("CBLogic performed the 'move' action");
                    CBAction(CBActionTypes.MOVE);
                    break;

                case CBActionTypes.TURNLEFT:
                    Debug.Log("CBLogic performed the 'turn left' action");
                    CBAction(CBActionTypes.TURNLEFT);
                    break;

                case CBActionTypes.TURNRIGHT:
                    Debug.Log("CBLogic performed the 'turn right' action");
                    CBAction(CBActionTypes.TURNRIGHT);
                    break;

                case CBActionTypes.JUMP:
                    Debug.Log("CBLogic performed the 'jump' action");
                    CBAction(CBActionTypes.JUMP);
                    break;
                default:
                    Debug.LogError("CBLogic: Failed to identify action: " + action);
                    break;
            }

            yield return new WaitForSeconds(actionDelay);
        }

        isExecutingQueue = false;
    }

    /// <summary>
    /// reads action blocks and returns a list of actions
    /// When adding a new action needs to be updated here.
    /// </summary>
    /// <returns>
    /// the list of actions to be performed
    /// </returns>
    List<CBActionTypes> GetActions()
    {
        List<CBActionTypes> actionList = new List<CBActionTypes>();
        int blockCount = 0;
        foreach (CodeBlock block in ActionBlockObjects)
        {
            List<String> blockActions = block.GetActions();
            int actionCount = 0;
            
            foreach (string action in blockActions)
            {
                switch (action)
                {
                    case "Move":
                        actionList.Add(CBActionTypes.MOVE);
                        Debug.Log("Code Block at position:" + blockCount + " contained: Action Move");
                        break;
                    case "TurnLeft":
                        actionList.Add(CBActionTypes.TURNLEFT);
                        Debug.Log("Code Block at position:" + blockCount + " contained: Action Turn Left");
                        break;
                    case "TurnRight":
                        actionList.Add(CBActionTypes.TURNRIGHT);
                        Debug.Log("Code Block at position:" + blockCount + " contained: Action Turn Right");
                        break;
                    case "Jump":
                        actionList.Add(CBActionTypes.JUMP);
                        Debug.Log("Code Block at position:" + blockCount + " contained: Action Jump");
                        break;
                    default:
                        Debug.LogError("Code Block at position:" + blockCount + " contains a erroneous action at point: " + actionCount);
                        break;
                }
                actionCount++;
            }
        }

        return actionList;
    }

    /// <summary>
    /// Performs the selected action - when adding a new action the behaviour must be specified here.
    /// </summary>
    /// <param name="CBActionType">
    /// the action to be performed
    /// </param>
    public void CBAction(CBActionTypes CBActionType)
    {
        // move player
        if (activePlayer == null)
        {
            Debug.LogError("CBLogic: Active player is not assigned!");
            return;
        }

        switch (CBActionType)
        {
            case CBActionTypes.MOVE:
                activePlayer.MovePlayerByFacing();
                break;
            case CBActionTypes.TURNLEFT:
                activePlayer.TurnPlayer(false);
                break;
            case CBActionTypes.TURNRIGHT:
                activePlayer.TurnPlayer(true);
                break;
            case CBActionTypes.JUMP:
                activePlayer.JumpPlayerByFacing();
                break;
            default:
                Debug.LogError("Something went wrong in CBAction in Game Controller - CBLogic.");
                break;
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
        return false;
    }

    void updateActionQueue(List<CodeBlock> currentQueue)
    {
        ActionBlockObjects = currentQueue;
    }

}