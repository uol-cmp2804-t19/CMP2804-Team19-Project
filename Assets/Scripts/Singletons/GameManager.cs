using UnityEngine;
using Config;
using System;

/// <summary>
/// Singleton that persists across all scenes and can be accessed from where-ever.
/// 'Owns' the configData.
/// Usage:
///  Access/Update:
///     GameManager.Main.Config.LevelsCompleted++;
///     GameManager.Main.Config.BestTimes["Level1"] = 300;
///  Save:
///     GameManager.Main.SaveConfig();
/// </summary>

public class GameManager : MonoBehaviour
{
    // removed extraneous game states to avoid pause/settings state confusion - we only care whether we're at title menu or in active game
    public enum GAME_STATE { INITIAL, TITLE_MENU, GAME_ACTIVE, TRANSITION}
    public GAME_STATE current_game_state = GAME_STATE.INITIAL;

    public static GameManager Main;
    public ConfigData Config;

    void Awake()
    {
        if (Main == null)
        {
            Main = this;
            DontDestroyOnLoad(gameObject);
            LoadConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadConfig()
    {
        try
        {
            string json = FileIO.ReadFile("config.json");
            Config = ConfigReader.FromJson(json);
        }
        catch
        {
            // if config doesn't exist create and save a default config to prevent errors
            Debug.LogError("No config.json found. Creating default config.");
            Config = new ConfigData();
            SaveConfig();
        }
    }

    public void SaveConfig()
    {
        FileIO.WriteFile("config.json", ConfigReader.ToJson(Config));
    }
}