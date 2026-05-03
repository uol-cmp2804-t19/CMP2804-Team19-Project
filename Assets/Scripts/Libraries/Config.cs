using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

// (On Windows) Config is saved to C:\Users\<YourName>\AppData\LocalLow\<CompanyName>\<ProductName>\config.json
// In Unity you can find <CompanyName> under edit/project_settings/player

/// <summary>
/// Config class library to manage metrics and configuration settings for the player.
/// Used alongside FileIO class library to save and load metrics for the game.
/// Usage:
///  Read:
///    string json = FileIO.ReadFile("config.json");
///    ConfigData config = ConfigReader.FromJson(json);
///  Access/Update:
///    config.LevelBestTimes["Level1"] = 300;
///    Debug.Log(config.LevelBestTimes["Level1"]);
///    config.LevelBestTimes["Level1"] = 300;
///    Debug.Log(config.LevelBestTimes["Level1"]);
///  Write:
///   string json = JsonConvert.SerializeObject(config);
///   FileIO.WriteFile("config.json", json);
/// </summary>

// TODO is configData saved on crash/force exit? saving at intervals may be necessary to prevent loss
namespace Config
{
    public class ConfigData
    {

        // basic metrics recorded including:
        //  - level name/best time in ticks
        //  - level name/best score
        //  - level name/best actions (total blocks in queue to complete level)
 
        //TODO implement game recording level time & score on level completion, feeding back to completion screen/config
        //TODO make sure levels have a key for config (use prefab filename?) to indicate completion
        //TODO may need to have a levelData class to track with level select, or use configData?
        public Dictionary<string, float> LevelBestTimes = new Dictionary<string, float>();
        public Dictionary<string, int> LevelBestScores = new Dictionary<string, int>();
        public Dictionary<string, int> LevelBestActions = new Dictionary<string, int>();

        public Dictionary<string, bool> LevelsCompleted = new Dictionary<string, bool>();


        // settings/player preferences
        public bool setting_isMuted = false;
        public float setting_volume = 1.0f;

        // constructor
        public ConfigData(
            Dictionary<string, float> newLevelBestTimes = null,
            Dictionary<string, int> newLevelBestScores = null,
            Dictionary<string, int> newLevelBestActions = null,
            Dictionary<string, bool> newLevelsCompleted = null,
            bool newSettingIsMuted = false,
            float newSettingVolume = 1.0f
            )
        {
            LevelBestTimes = newLevelBestTimes ?? new Dictionary<string, float>();
            LevelBestScores = newLevelBestScores ?? new Dictionary<string, int>();
            LevelBestActions = newLevelBestActions ?? new Dictionary<string, int>();
            LevelsCompleted = newLevelsCompleted ?? new Dictionary<string, bool>();
            setting_isMuted = newSettingIsMuted;
            setting_volume = newSettingVolume;
        }
    }

    public class ConfigReader
    {
        // Reads a JSON string and converts to ConfigData object - create default if invalid
        public static ConfigData FromJson(string jsonString)
        {
            try
            {
                ConfigData data = JsonConvert.DeserializeObject<ConfigData>(jsonString);
                return data ?? new ConfigData();
            }
            catch
            {
                Debug.LogError("Config Reader could not read the JSON");
                return new ConfigData();
            }
        }

        // Converts ConfigData to JSON
        public static string ToJson(ConfigData data)
        {
            return JsonConvert.SerializeObject(data);
        }

    }
}