using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public static ConfigData FromJson(string jsonString)
        {
            ConfigData data = JsonConvert.DeserializeObject<ConfigData>(jsonString);
            return data ?? new ConfigData();
        }
    }
}