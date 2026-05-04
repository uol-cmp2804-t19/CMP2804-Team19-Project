using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Numerics;

// ambient level sound - https://freesound.org/search/?q=ambient+forest
// first level bgm - https://opengameart.org/content/music-for-your-first-level
// research game soundtracks like BabaIsYou

/// <summary>
/// Level manager controls the separate tilemap z-levels and player movement
/// </summary>
public class LevelMapManager : MonoBehaviour {

    public LevelLayer activeLayer = null;
    public PlayerController player = null;

    public bool is_level_active = false;

    Dictionary<int, LevelLayer> mapLayerRegister = new Dictionary<int, LevelLayer>();
    
    // metrics fed back to configData
    public string levelName = "undefined_level";
    public int levelScore = 0;
    public float levelTimeSeconds = 0f;
    public int blockQueueSize = 0;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// This is the level onLoad logic and where CBLogic needs to be connected
    /// </summary>
    void Start()
    {
        _setupMapLayer();
        _setupPlayerOnMap();
    }

    void Update() {
        if (is_level_active) {
            levelTimeSeconds += Time.deltaTime; // seconds
        }
    }

    public void ActivateLevel() {
        is_level_active = true;
        gameObject.SetActive(true);
        Debug.Log("New level activated: " + levelName);
    }

    public void AddScore(int score_change)
    {
        levelScore += score_change;
    }

    public void SetBlockQueueSize(int newSize)
    {
        blockQueueSize = newSize;
    }

    /// <summary>
    /// Changes the active map layer to the one corresponding to the given z-level, if it exists in the register
    /// Called during player jump behaviour
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
            LevelLayer newLayer = mapLayerRegister[newZLevel];
            activeLayer = newLayer;
        }
        

    }
     public void DeactivateLevel() {
        is_level_active = false;
        gameObject.SetActive(false);
        Debug.Log("Level deactivated! (" + levelName + ")");
    }

    // doesn't rely on cell existing in layer
    public Vector3Int GetPlayerCell()
    {
        if (player == null || activeLayer == null)
        {
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
    public bool hasTileAtCell(Vector3Int cellPosition)
{
    if (player == null)
    {
        Debug.Log("You forgot to assign a player and/or map!");
        return false;
    }

    // x/y for tilemap lookup, z is nil because not storing z layer by tilemap
    Vector3Int tileCell = new Vector3Int(cellPosition.x, cellPosition.y, 0);

    if (mapLayerRegister.ContainsKey(cellPosition.z))
    {
        if (mapLayerRegister[cellPosition.z].tilemap.HasTile(tileCell))
        {
            return true;
        }
    }

    // else
    Debug.Log("No tile found at cell " + tileCell + " on z-level " + cellPosition.z);
    return false;
}

// confirm is movement is blocked by cell on higher layer
public bool isBlockedByElevation(Vector3Int cellPosition) {
    if (player == null)
    {
        Debug.Log("You forgot to assign a player and/or map!");
        return true;
    }

    // x/y for tilemap lookup, z is nil because not storing z layer by tilemap
    Vector3Int tileCell = new Vector3Int(cellPosition.x, cellPosition.y, 0);

    // blocked if any layer ABOVE intended z has a tile at target x/y
    foreach (var (key, value) in mapLayerRegister)
    {
        if (key > cellPosition.z)
        {
            if (value.tilemap.HasTile(tileCell))
            {
                Debug.Log("Move blocked by tile on higher layer " + value.name + " at z-level " + key);
                return true;
            }
        }
    }

    // else
    return false;
}

public int getHighestValidLayer(Vector3Int cellPosition) {
    // find highest tile at or below intended z
    int highestLayer = -1;

    // start at highest layer work down, if above the target layer then ignore it
    // call hasTileAtCell on each layer, then get highest existing
    // once highest existing found at or below target layer, check if anything higher with isBlockedByElevation
    // If not then return that layer as valid move, if so return -1 (invalid move)

    foreach (var (key, value) in mapLayerRegister)
    {
        if (key > cellPosition.z)
        {
            continue;
        }
        else
        {
            if (hasTileAtCell(new Vector3Int(cellPosition.x, cellPosition.y, key)) && key > highestLayer)
            {
                highestLayer = key;
            }
        }
    }

    if (highestLayer == -1)
    {
        Debug.Log("No valid layers found at or below target layer " + cellPosition.z + " for cell " + cellPosition);
        return -1;
    }
    else
    {
        if (isBlockedByElevation(new Vector3Int(cellPosition.x, cellPosition.y, highestLayer)))
        {
            Debug.Log("Move blocked by tile on higher layer at z-level " + cellPosition.z);
            return -1;
        }
        else
        {
            return highestLayer;
        }
    }

}

    /// <summary>
    /// Moves the player in the specified direction.
    /// </summary>
    /// <param name="direction">
    /// The direction to move the player, where (0, 1) is up, (0, -1) is down, (-1, 0) is left, and (1, 0) is right. Handled by facing logic.
    /// </param>
    public void MovePlayerOnGrid(Vector3Int position_change)
    {
        // 'position_change' was 'direction', now represents 3d change in cell, z is 0 if walking or 1 if jumping

        // BEHAVIOUR
        // Check player/layer valid
        // Get target cell from player cell
        // note - Check if tile exists behaviour currently in 'teleport to cell' func but should be moved out
        // FLOW FOR VALIDITY
        // if new z == current z, is valid if tile exists on z level or at lower level & no tile exists on higher levels
        // if new z > current z, is valid if tile exists on higher z level, current z level, lower z level... & no tile exists on higher than new z levels
        // this is effectively the same
        // just get the highest valid layer, move there, update player z level
        
        if (player == null || activeLayer == null)
        {
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }
        else
        {
            // add nil z because not updating zLayer here
            Vector3Int targetCell = GetPlayerCell() + new Vector3Int(
                Mathf.RoundToInt(position_change.x),
                Mathf.RoundToInt(position_change.y),
                Mathf.RoundToInt(position_change.z)
            );

            int targetLayer = getHighestValidLayer(targetCell);
            if (targetLayer == -1) {
                Debug.Log("No valid target layer found for move to cell " + targetCell);
                return;
            }
            else
            {
                player.zLevel = targetLayer;
                changeActiveMap(player.zLevel);
                // temporary teleport/snapping behaviour, to animate gradually eventually
                TeleportPlayerToCell(new Vector3Int(targetCell.x, targetCell.y, player.zLevel));
            }
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
        if (GameManager.Main.Config == null) {
            Debug.Log("No config data found to save level metrics to!");
            return;
        }

        // overwrite level name in configData as complete
        GameManager.Main.Config.LevelsCompleted[levelName] = true;
        
        // overwrite level best time if new time is better or no existing time
        if (GameManager.Main.Config.LevelBestTimes.ContainsKey(levelName)) {
            float recorded_best_time = GameManager.Main.Config.LevelBestTimes[levelName];
            if (levelTimeSeconds < recorded_best_time || recorded_best_time == 0) {
                GameManager.Main.Config.LevelBestTimes[levelName] = levelTimeSeconds;
            }
        } else {
            GameManager.Main.Config.LevelBestTimes[levelName] = levelTimeSeconds;
        }

        // overwrite level best score if new score is better or no existing score
        if (GameManager.Main.Config.LevelBestScores.ContainsKey(levelName)) {
            int recorded_best_score = GameManager.Main.Config.LevelBestScores[levelName];
            if (levelScore > recorded_best_score || recorded_best_score == 0) {
                GameManager.Main.Config.LevelBestScores[levelName] = levelScore;
            }
        } else {
            GameManager.Main.Config.LevelBestScores[levelName] = levelScore;
        }

        // overwrite level best actions if new action count is better or no existing action count
        if (GameManager.Main.Config.LevelBestActions.ContainsKey(levelName)) {
            int recorded_best_blocks = GameManager.Main.Config.LevelBestActions[levelName];
            if (blockQueueSize < recorded_best_blocks || recorded_best_blocks == 0) {
                GameManager.Main.Config.LevelBestActions[levelName] = blockQueueSize;
            }
        } else {
            GameManager.Main.Config.LevelBestActions[levelName] = blockQueueSize;
        }
    }

    // plays sound & snaps player to cell
    void TeleportPlayerToCell(Vector3Int targetCell)
    {
        if (player == null || activeLayer == null) {
            Debug.Log("You forgot to assign a player and/or map!");
            return;
        }

        // snap to grid based on target cell, not player cell, to avoid compounding errors from multiple moves
        Vector3Int tileCell = new Vector3Int(targetCell.x, targetCell.y, 0);
        Vector3Int player_cell = activeLayer.tilemap.WorldToCell(player.transform.position);
        Vector3 cell_half_size = activeLayer.tilemap.cellSize / 2f;
        Vector3 player_pos = activeLayer.tilemap.CellToWorld(tileCell) + cell_half_size;

        player.transform.position = player_pos;
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
            // WARNING - this has no validation for cell existing
            // TODO add logic to find nearest valid cell instead
            // TODO this is temporary because player is offset from editor positioning
            TeleportPlayerToCell(GetPlayerCell());
        }
    }
