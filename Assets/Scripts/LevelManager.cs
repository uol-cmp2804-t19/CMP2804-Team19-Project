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
/// </summary>
public class LevelMapManager : MonoBehaviour {

    public LevelLayer activeLayer = null;
    public PlayerController player = null;

    Dictionary<int, LevelLayer> mapLayerRegister = new Dictionary<int, LevelLayer>();
    
    // metrics fed back to configData
    public string levelName = "undefined_level";
    int levelScore = 0;
    int levelTime = 0;
    int blockQueueSize = 0;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// This is the level onLoad logic and where CBLogic needs to be connected
    /// </summary>
    void Start()
    {
        _setupMapLayer();
        _setupPlayerOnMap();
    }

    public void AddScore(int score_change)
    {
        levelScore += score_change;
    }

    /// <summary>
    /// Changes the active map layer to the one corresponding to the given z-level, if it exists in the register
    /// TODO - this should be called with jump before movePlayerInDirection, as player z-level is not automatically updated by that function
    /// </summary>
    public void changeActiveMap(int newZLevel)
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

    // doesn't rely on cell existing in layer
    public Vector3Int GetPlayerCell()
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
    public bool isValidMove(Vector3Int cellPosition)
    {
        if (player == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return false;
        }
        // check if tile exists on current layer
        // if jumping layer should be updated first (call before movePlayerInDirection) so player z-level is correct for this check
        if (mapLayerRegister.ContainsKey(cellPosition.z)) {
            LevelLayer checkLayer = mapLayerRegister[cellPosition.z];
            if (checkLayer.tilemap.HasTile(cellPosition)) {
                // tile exists, now check if we're blocked by higher layers
                // check all layers in MapLayerRegister with higher z-levels than the target cell for a tile at the same x,y position, if any exist then move is invalid
                foreach (var (key, value) in mapLayerRegister) {
                    if (key > cellPosition.z) {
                        if (value.tilemap.HasTile(cellPosition)) {
                            Debug.Log("Move blocked by tile on higher layer " + value.name + " at z-level " + key);
                            return false;
                        }
                    }
                }
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
    /// TODO - if jumping call alongside changeActiveMap to move between layers, as player z-level is not automatically updated by this function
    /// </summary>
    /// <param name="direction">
    /// The direction to move the player, where (0, 1) is up, (0, -1) is down, (-1, 0) is left, and (1, 0) is right. Handled by facing logic.
    /// </param>    
    public void MovePlayerInDirection(Vector2 direction)
    {
        if (player == null || activeLayer == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        else
        {
            //TODO - currently player can move diagonally, this will not be possible with coding block calls - does it need to be captured here?
            // add nil z because not updating zLayer here
            Vector3Int targetCell = GetPlayerCell() + new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            // temporary teleport/snapping behaviour, to animate gradually eventually
            TeleportPlayerToCell(targetCell);
        }
    }

    // disabled for release build
    // // draw a red square around the player current cell, for debugging purposes - will appear in origin position until player first move
    // void OnDrawGizmos() {
    //     // silently fail, debug handling only
    //     if (player == null || activeLayer == null || activeLayer.tilemap == null) return;
    //     Vector3Int cell = activeLayer.tilemap.WorldToCell(player.transform.position);
    //     Vector3 center = activeLayer.tilemap.GetCellCenterWorld(cell);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(center, activeLayer.tilemap.cellSize);
    // }

    public void saveLevelMetricsToConfig() {
        if (gamemanager.Main.configData == null) {
            Debug.Log("No config data found to save level metrics to!");
            return;
        }
        
        //TODO implement saving level metrics to configData on level completion, called from completion screen

        // overwrite level name in configData as complete
        gamemanager.Main.configData.LevelsCompleted[levelName] = true;
        
        // overwrite level best time if new time is better or no existing time
        if (gamemanager.Main.configData.LevelBestTimes.ContainsKey(levelName) {
            int recorded_best_time = gamemanager.Main.configData.LevelBestTimes[levelName];
            if (levelTime < recorded_best_time || recorded_best_time == 0) {
                gamemanager.Main.configData.LevelBestTimes[levelName] = levelTime;
            }
        } else {
            gamemanager.Main.configData.LevelBestTimes[levelName] = levelTime;
        }

        // overwrite level best score if new score is better or no existing score
        if (gamemanager.Main.configData.LevelBestScores.ContainsKey(levelName) {
            int recorded_best_score = gamemanager.Main.configData.LevelBestScores[levelName];
            if (levelScore > recorded_best_score || recorded_best_score == 0) {
                gamemanager.Main.configData.LevelBestScores[levelName] = levelScore;
            }
        } else {
            gamemanager.Main.configData.LevelBestScores[levelName] = levelScore;
        }

        // overwrite level best actions if new action count is better or no existing action count
        if (gamemanager.Main.configData.LevelBestActions.ContainsKey(levelName) {
            int recorded_best_blocks = gamemanager.Main.configData.LevelBestActions[levelName];
            if (blockQueueSize < recorded_best_blocks || recorded_best_blocks == 0) {
                gamemanager.Main.configData.LevelBestActions[levelName] = blockQueueSize;
            }
        } else {
            gamemanager.Main.configData.LevelBestActions[levelName] = blockQueueSize;
        }
    }

    //TODO this is duplicated by playerController, one or the other needs to own this
    //TODO - this is currently a teleport, needs to be replaced with gradual movement and animation (especially for jumping) eventually
    //TODO is this respecting levelLayer Z Levels?
    void TeleportPlayerToCell(Vector3Int targetCell)
    {
        if (player == null || activeLayer == null)
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }

        if (!isValidMove(targetCell))
        {
            Debug.Log("Invalid move attempted to cell " + targetCell + " on z-level " + targetCell.z);
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

        // bugged
        // List<LevelLayer> mapLayers = GameObject.FindGameObjectsWithTag("LevelLayer").ToList<LevelLayer>();
        // for (int i = 0; i < mapLayers.Count; i++)
        // {
        //     LevelLayer layer = mapLayers[i];
        //     mapLayerRegister[layer.zLevel] = layer;
        //     Debug.Log("LevelLayer '" + layer.name + "' added to LevelMapManager via automated find");
        // }

        var map_layers = FindObjectsByType<LevelLayer>(FindObjectsSortMode.None);
        foreach (var layer_object in map_layers)
        {
            LevelLayer layerComp = layer_object.GetComponent<LevelLayer>();
            if (layerComp != null)
            {
                // will overwrite if multiple layers have the same zLevel, but this is a user error and should be caught in editor
                mapLayerRegister[layerComp.zLevel] = layerComp;
                Debug.Log("LevelLayer '" + layerComp.name + "' added to LevelMapManager via automated find");
            }
        }
        foreach (var (key, value) in mapLayerRegister)
        {
            // set sorting order by specified ZLevel (offset by 1), forcing levellayers with duplicate ZLevels to default to nil & not render
            if (value != null || value is LevelLayer) {
                if (value.tilemap == null) {
                    Debug.Log("No tilemap found on LevelLayer '" + value.name + "', cannot set sorting order!");
                    continue;
                }
                TilemapRenderer renderer = value.tilemap.GetComponent<TilemapRenderer>();
                if (renderer == null)
                {
                    Debug.Log("No TilemapRenderer found on LevelLayer '" + value.name + "', cannot set sorting order!");
                    continue;
                }
                // else
                renderer.sortingOrder = key+1;
            }
            else
            {
                Debug.Log("No LevelLayer found for z-level " + key + ", cannot set sorting order!");
            }
            Debug.Log("Registered map layer for z-level " + key + ": " + value.name);
            
        }
        Debug.Log("MLR=\n"+mapLayerRegister);
    }

    private void _setupPlayerOnMap() {
        // Initial player setup within the level if not manually assigned
        if (player == null)
        {
            // search for child with tag player, player gets reloaded with level
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            if (player != null) {
                Debug.Log("Player successfully added to LevelMapManager via automated find");
            } else {
                Debug.Log("Player must be manually assigned to LevelMapManager as cannot be found - does it exist within level scene?"); 
                return;
            }
        }

        // Player startup
        // Set the initial active map layer to player z-level
        changeActiveMap(player.zLevel);
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