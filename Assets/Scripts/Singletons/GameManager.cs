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
            Debug.LogError("LoadConfig in GameManager called with no config.json file present");
        }
    }

    public void SaveConfig()
    {
        FileIO.WriteFile("config.json", ConfigReader.ToJson(Config));
    }
}