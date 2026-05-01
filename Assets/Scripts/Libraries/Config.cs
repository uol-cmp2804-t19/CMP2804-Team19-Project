using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq.Expressions;

/// Config class library to manage metrics and configuration settings for the player.
/// Used alongside FileIO class library to save and load metrics for the game.
/// Usage:
///  Read:
///    string json = FileIO.ReadFile("config.json");
///    ConfigData config = ConfigReader.FromJson(json);
///  Access/Update:
///    config.LevelsCompleted += 1;
///    config.BestTimes["Level1"] = 300;
///    Debug.Log(config.LevelsCompleted);
///    Debug.Log(config.BestTimes["Level1"]);
///  Write:
///   string json = JsonConvert.SerializeObject(config);
///   FileIO.WriteFile("config.json", json);

namespace Config
{
    [Serializable]
    public class ConfigData
    {
        // basic metrics recorded including collection of level name/best time in ticks
        public int LevelsCompleted = 0;
        public Dictionary<string, int> BestTimes = new Dictionary<string, int>();

        // constructor
        public ConfigData(
            int newLevelsCompleted = 0,
            Dictionary<string, int> newBestTimes = null)
        {
            LevelsCompleted = newLevelsCompleted;
            BestTimes = newBestTimes ?? new Dictionary<string, int>();
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