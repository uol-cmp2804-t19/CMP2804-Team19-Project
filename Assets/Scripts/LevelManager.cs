using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

//TODO add an audio manager to choose sound effect based on terrain and modulate pitch/choose from sound array
// ambient level sound - https://freesound.org/search/?q=ambient+forest
// first level bgm - https://opengameart.org/content/music-for-your-first-level
// research game soundtracks like BabaIsYou

//TODO animate player walk
//TODO make player move/jump to cell gradually rather than snap to - replace moveDelay check with 'is player animating' and queue actions?
//TODO look at separation of concerns - should player move/jump-to cell be a palyer method?

/// <summary>
/// Level manager controls the separate tilemap z-levels and player movement
public class LevelMapManager : MonoBehaviour {

    Dictionary<int, LevelLayer> mapLayerRegister = new Dictionary<int, LevelLayer>();
    LevelLayer activeLayer = null;

    public PlayerController player = null;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// This is the level onLoad logic and where CBLogic needs to be connected
    /// </summary>
    void Start()
    {
        _setupMapLayer();
        _setupPlayerOnMap();
    }

    // doesn't rely on cell existing in layer
    Vector3Int GetPlayerCell()
    {
        if (player == null || activeLayer == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return new Vector3Int(0, 0, 0);
        }
        else
        {
            Vector3Int currentCell = activeLayer.tilemap.WorldToCell(player.transform.position);
            currentCell.z = activeLayer.zLevel;
            return currentCell;
        }
    }

    /// <summary>
    /// Checks if the player can move to the specified cell position on the target map layer
    /// </summary>
    /// <param name="cellPosition">
    /// The cell position to check for validity - Z should be the ZLayer argument
    /// </param>
    /// <returns>
    /// True if the move is valid, false otherwise
    /// </returns>
    bool isValidMove(Vector3Int cellPosition) {
        if (mapLayerRegister.ContainsKey(cellPosition.z)) {
            LevelLayer checkLayer = mapLayerRegister[cellPosition.z];
            if (checkLayer.tilemap.HasTile(cellPosition)) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            Debug.Log("No map layer registered for z-level " + player.zLevel);
            return false;
        }
    }

    /// <summary>
    /// Moves the player in the specified direction.
    /// </summary>
    /// <param name="direction">
    /// The direction to move the player, where (0, 1) is up, (0, -1) is down, (-1, 0) is left, and (1, 0) is right. Handled by facing logic.
    /// </param>    
    void MovePlayerInDirection(Vector2 direction)
    {
        if (player == null || tilemap == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        else
        {
            //TODO - currently player can move diagonally, this will not be possible with coding block calls - does it need to be captured here?
            Vector3Int targetCell = GetPlayerCell() + new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            // temporary teleport/snapping behaviour, to animate gradually eventually
            TeleportPlayerToCell(targetCell);
        }
    }

    /// <summary>
    /// Changes the active map layer to the one corresponding to the given z-level, if it exists in the register
    /// </summary>
    void OnChangeActiveMap(int newZLevel)
    {
        if (!mapLayerRegister.ContainsKey(newZLevel))
        {
            Debug.Log("No map layer registered for z-level " + newZLevel);
            return;
        }
        else
        {
            //TODO check if valid beforehand
            //TODO add transition animation effect (level fade?)
            LevelLayer newLayer = mapLayerRegister[newZLevel];
            activeLayer = newLayer;
        }
    }

    // draw a red square around the player current cell, for debugging purposes - will appear in origin position until player first move
    void OnDrawGizmos() {
        // silently fail, debug handling only
        if (player == null || activeLayer == null) return;
        Vector3Int cell = activeLayer.WorldToCell(player.transform.position);
        Vector3 center = activeLayer.GetCellCenterWorld(cell);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, activeLayer.tilemap.cellSize);
    }

    //TODO - this is currently a teleport, needs to be replaced with gradual movement and animation (especially for jumping) eventually
    void TeleportPlayerToCell(Vector3Int targetCell)
    {
        if (player == null || activeLayer == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }

        if (!activeLayer.tilemap.HasTile(targetCell))
        {
            Debug.Log("off map");
            return;
        }

        // snap to grid
        player.transform.position =
        activeLayer.tilemap.GetCellCenterWorld(activeLayer.tilemap.WorldToCell(player.transform.position)
        );
        //player.transform.position = activeLayer.tilemap.GetCellCenterWorld(targetCell);
        // move player to center of tile
        player.transform.position = activeLayer.tilemap.CellToWorld(targetCell) + activeLayer.tilemap.cellSize / 2f;
        Debug.Log("on map");
        player.PlayWalkSound();

    }

    /* #####################################################################################################################
    // private functions
    ##################################################################################################################### */

    private void _setupMapLayer() {
        // Map layer setup
        List<LevelLayer> mapLayers = GameObject.FindGameObjectsWithTag("LevelLayer").ToList<LevelLayer>();
        for (int i = 0; i < mapLayers.Count; i++)
        {
            // will overwrite if multiple layers have the same zLevel, but this is a user error and should be caught in editor
            LevelLayer layer = mapLayers[i];
            mapLayerRegister[layer.zLevel] = layer;
            Debug.Log("LevelLayer '" + layer.name + "' added to LevelMapManager via automated find");
        }
    }

    private void _setupPlayerOnMap() {
        // Initial player setup within the level
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            Debug.Log("Player successfully added to LevelMapManager via automated find");
        }
         else {
            Debug.Log("Player must be manually assigned to LevelMapManager as cannot be found - does it exist within level scene?"); 
         }

        // Player startup
        // Set the initial active map layer to player z-level
        OnChangeActiveMap(player.zLevel);
        if (activeLayer == null)
        {
            Debug.Log("No active layer found for z-level " + player.zLevel + ", please check your map layers!");
        }
        else
        {
            // Immediately lock player to their current grid cell
            // TODO this is temporary because player is offset from editor positioning
            TeleportPlayerToCell(GetPlayerCell());
        }
    }

}