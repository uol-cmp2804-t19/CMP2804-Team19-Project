using System;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // game objects are declared here so they can be manually added
    public GameObject PlayerCharacter = null;
    private Rigidbody2D PlayerCharacterRB; // declared here for movement

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


    }
}
