using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CBLogic : MonoBehaviour
{
    public List<GameObject> ActionBlocks = new List<GameObject>();
    public GameObject Action1 = null;
    public GameObject Action2 = null;
    public GameObject Action3 = null;


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

        if (Action1.tag == "Untagged")
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

        if (Action2.tag == "Untagged")
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

        if (Action3.tag == "Untagged")
        {
            Action3.tag = "ActionEmpty";
            Debug.Log("'ActionEmpty' added to Action block 3");
        }

    }
    // currently seperate from start for testing purposes, will add once guarenteed to work.
    void AddActionBlocks()
    {
        
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
                        break;
                    case CBActionTypes.down:
                        Debug.Log("CBLogics performed the 'down' action");
                        break;
                    case CBActionTypes.left:
                        Debug.Log("CBLogics performed the 'left' action");
                        break;
                    case CBActionTypes.right:
                        Debug.Log("CBLogics performed the 'right' action");
                        break;
                    default:
                        Debug.LogError("'PerformActions' in CBLogic failed to identify: " + action);
                        break;
                }
            }
        }

        CBAction(CBActionTypes.up);
    }

    List<CBActionTypes> GetActions()
    {
        List<CBActionTypes> actionList = new List<CBActionTypes>();

        return actionList;
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
        none
    }
}
