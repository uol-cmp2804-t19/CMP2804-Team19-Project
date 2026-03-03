using UnityEngine;
using Config;

/// Singleton that persists across all scenes and can be accessed from where-ever.
/// 'Owns' the configData.

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
        string json = FileIO.ReadFile("config.json");
        Config = ConfigReader.FromJson(json);
    }

    public void SaveConfig()
    {
        FileIO.WriteFile("config.json", ConfigReader.ToJson(Config));
    }
}