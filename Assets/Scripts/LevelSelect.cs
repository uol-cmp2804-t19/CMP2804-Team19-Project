using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Public Methods
/// onStartButtonPressed() - for start button, tells bootstrap to load level from resource path
/// onExitButtonPressed() - for exit button, returns to title menu
/// openMenu() - opens this menu, needs special handling for populating/destroying don't just set active
/// </summary>

/// <summary>
// Private Methods
// clearLevels() - clears level data and buttons to prevent duplicates, called at startup and on exit to confirm
// closeMenu() - needs special handling for populating/destroying don't just set inactive, shouldn't be called externally, only by buttons
// populateLevelsFromDisk() - loading function part 1, builds levelData from config & resourcePath prefab level names
// buildLevelSelectButtons() - loading function part 2, builds tree objects
// updateLevelInfo() - called whenever button clicked to update info panel
// ensureLevelConfigExists() - called to populate config with default values
/// </summary>

// Script Behaviour
/// <summary>
/// This script attaches to level select menu has a leftmost field/container that lists levels as buttons vertically
/// On rightmost side is a field for level info and, below it, start level/exit menu (return to title) buttons
/// Behaviour of this script:
/// -   populating level select buttons from prefabs in resources
/// -   loading level on selection and passing prefab to game manager
/// -   displaying level metrics on info panel
/// -   informing game manager to start level on start
/// -   returning to title menu on exit
/// </summary>
public class LevelSelectManager : MonoBehaviour {

    // data container for levels, populated from levelname on the prefab & respective config data
    // loaded from config when level buttons are populated, to organise data & limit file reads to menu open
    // button list is loaded when menu is opened, performance not a concern as this doesn't happen frequently
    struct LevelData
    {
        // for loading level_instance, add to scene and set inactive then pass ref to bootstrap_scene
        public string resourcePath;
        // name displays on button, other metrics on info panel (time stored as processed ticks, needs to be converted for display)
        public string levelName;
        public int bestScore;
        public float bestTime;
        public int bestQueueSize;
    }

    // bootstrap is the scene controller handling prefab loading
    public GameObject bootstrap_scene = null;
    private main_bootstrap bootstrap_script = null;

    // following references must be set in editor
    // -    container is the left-most field container buttons
    // -    prefab is the testing button prefab, should be set inactive, will be duplicated to make level buttons
    // -        (this button requires child with text component)
    // -    info panel refs are for text labels in the right-most field displaying level metrics
    //          (these text labels will be updated when level button clicked)

    public GameObject level_button_container = null;
    public GameObject level_button_prefab = null;
    // info panel text labels
    public GameObject level_info_name = null;
    public GameObject level_info_score = null;
    public GameObject level_info_time = null;
    public GameObject level_info_queue = null;

    // scene buttons need to be connected to onStartButtonPressed & onExitButtonPressed respectively
    public GameObject button_start_level = null;
    public GameObject button_exit_menu = null;

    // for removing the button due to a resetting levels bug
    private GameObject active_level_button_selected = null;

    // this is the level most recently clicked, should default to topmost or null if no levels exist (error)
    // is changed when the respective level button is clicked
    LevelData active_level_selected = new LevelData
    {
        resourcePath = "",
        levelName = "",
        bestScore = 0,
        bestTime = 0.0f,
        bestQueueSize = 0
    };

    // store all loaded levels
    LevelData[] all_levels = new LevelData[0];

    private void Start()
    {
        getBootstrapScriptComponent();
    }

    public void openMenu()
    {
        // set the menu active
        gameObject.SetActive(true);
        // confirm exists
        getBootstrapScriptComponent();

        // populate level select buttons from Resources/GameLevels
        // -    for each prefab, load level name from config and display on button
        // -    for each prefab, load level metrics from config and display on info panel on button hover
        // -    add listener to button to reference file when clicked and pass prefab to game manager
        // -    add listener to button to display level metrics on info panel when clicked

        // load levels, set buttons, update metric display to initial option
        populateLevelsFromDisk();
        buildLevelSelectButtons();
        if (getIfInitialLevel() == true)
        {
            updateLevelInfo();
        }
    }

    private bool getIfInitialLevel()
    {
        if (all_levels.Length > 0) {
            active_level_selected = (LevelData)all_levels[0];

            // for confidence
            level_info_name.SetActive(true);
            level_info_score.SetActive(true);
            level_info_queue.SetActive(true);
            level_info_time.SetActive(true);

            return true;
        }
        else
        {
            Debug.LogError("No levels loaded cannot set level info, disabling levelInfo!");
            level_info_name.SetActive(false);
            level_info_score.SetActive(false);
            level_info_queue.SetActive(false);
            level_info_time.SetActive(false);

            return false;
        }
    }

    public void updateLevelInfo() {

        Debug.Log("updateLevelInfo called for: " + active_level_selected.levelName);
        Debug.Log("Level name: " + active_level_selected.levelName + "Level info - score: " + active_level_selected.bestScore + " time: " + active_level_selected.bestTime + " queue: " + active_level_selected.bestQueueSize);

        if (active_level_selected.levelName == "")
        {
            Debug.LogError("Active level not set cannot update info!");
            return;
        }

        if (level_info_name == null ||
            level_info_queue == null ||
            level_info_score == null ||
            level_info_time == null)
        {
            Debug.LogError("Refs not set for UpdateLevelInfo;" +
                " name (" + level_info_name + "), " +
                " queue (" + level_info_queue + "), " +
                " score (" + level_info_score + "), " +
                " time (" + level_info_time + ")."
                );
            return;
        }

        TextMeshProUGUI nameText = level_info_name.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI scoreText = level_info_score.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeText = level_info_time.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI queueText = level_info_queue.GetComponent<TextMeshProUGUI>();

        if (nameText == null || scoreText == null || timeText == null || queueText == null)
        {
            Debug.LogError("One or more level info objects is missing a TextMeshProUGUI component.");
            return;
        }

        nameText.text = active_level_selected.levelName;
        scoreText.text = "Best Score: " + active_level_selected.bestScore;
        timeText.text = "Best Time: " + active_level_selected.bestTime.ToString("F2") + "s";
        queueText.text = "Best Queue Size: " + active_level_selected.bestQueueSize;

    }
    private void buildLevelSelectButtons()
    {
        if (level_button_container == null || level_button_prefab == null)
        {
            Debug.LogError("LevelSelectManager: level button container and/or prefab not set in inspector!");
            return;
        }

        foreach (LevelData levelData in all_levels)
        {
            GameObject button = Instantiate(level_button_prefab, level_button_container.transform);

            Debug.Log("attempting to set buttons for " + levelData.levelName);

            Button buttonComponent = button.GetComponent<Button>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

            if (buttonText == null || buttonComponent == null)
            {
                Debug.LogError("Level button prefab is missing Text and/or Button component;"
                    + " LevelButtonObject = " + button + " "
                    + " LevelButtonComponent = " + buttonComponent + " "
                    + " LevelButtonText = " + buttonText
                );
                Destroy(button);
                continue;
            }

            buttonText.text = levelData.levelName;

            buttonComponent.onClick.AddListener(() => {
                Debug.Log("Level button clicked: " + levelData.levelName + " path: " + levelData.resourcePath);
                active_level_selected = levelData;
                active_level_button_selected = button;
                updateLevelInfo();
            });

            button.SetActive(true);

            // set first generated button as initial selected button
            if (active_level_button_selected == null)
            {
                active_level_selected = levelData;
                active_level_button_selected = button;
            }

            Debug.Log("I have added " + buttonComponent + " to " + buttonComponent.transform.parent.name);
        }
    }

    private void getBootstrapScriptComponent()
    {
        if (bootstrap_scene != null)
        {
            bootstrap_script = bootstrap_scene.GetComponent<main_bootstrap>();
        }
        else
        {
            Debug.Log("bootstrap scene not set in editor!");
        }
    }

    // levels currently exist in ./Assets/Resources/GameLevels/
    private void populateLevelsFromDisk()
    {
        // make sure starting from clear slate
        all_levels = new LevelData[0];
        // build array of level data
        GameObject[] levelPrefabs = Resources.LoadAll<GameObject>("GameLevels");

        for (int i = 0; i < levelPrefabs.Length; i++)
        {
            // load level data from config using prefab name as key
            string prefabName = levelPrefabs[i].name;
            ensureLevelConfigExists(prefabName);

            // set to default values if level not found in configData
            float bestTime = GameManager.Main.Config.LevelBestTimes.ContainsKey(prefabName) ? GameManager.Main.Config.LevelBestTimes[prefabName] : 0f;
            int bestScore = GameManager.Main.Config.LevelBestScores.ContainsKey(prefabName) ? GameManager.Main.Config.LevelBestScores[prefabName] : 0;
            int bestQueueSize = GameManager.Main.Config.LevelBestActions.ContainsKey(prefabName) ? GameManager.Main.Config.LevelBestActions[prefabName] : 0;
            // when level is played and exited it will resave to config

            LevelData levelData = new LevelData
            {
                resourcePath = "GameLevels/" + prefabName,
                levelName = prefabName,
                bestScore = bestScore,
                bestTime = bestTime,
                bestQueueSize = bestQueueSize
            };
            
            // resize array to add new level data, add to array
            System.Array.Resize(ref all_levels, all_levels.Length + 1);
            all_levels[all_levels.Length - 1] = levelData;
        }

        Debug.Log("Loaded " + all_levels.Length + " levels!");

    }

    // connect start button to this
    // when level is selected (start pressed) need to tell bootstrap to actually load the level prefab (wait for it) & close this menu
    // bootstrap will handle transition to game state as this behaviour is owned by that
    public void onStartButtonPressed()
    {


        Debug.Log("Start button pressed. Selected path: " + active_level_selected.resourcePath);
        // check if valid path
        if (string.IsNullOrEmpty(active_level_selected.resourcePath))
        {
            Debug.LogError("No level selected!");
            return;
        }
        // else
        // pass the file path of the chosen level
        if (bootstrap_scene == null || bootstrap_script == null)
        {
            Debug.LogError("LevelSelectManager: bootstrap scene or script reference not set.");
            return;
        }

        //removed dynamic instanation due to race condition bug with player referencing
        //bootstrap_script.LoadLevel(active_level_selected.resourcePath);

        // to fix level replayability bug
        if (active_level_button_selected != null)
        {
            Debug.Log("disabling button for " + active_level_selected.levelName);
            active_level_button_selected.SetActive(false);
            //Destroy(active_level_button_selected);
            active_level_button_selected = null;
        }

        // level name should correspond to hardcoded if block
        bootstrap_script.startHardcodedLevel(active_level_selected.levelName);
        closeMenu();
    }

    // connect exit button to this
    // levelSelect is not part of game state it is a submenu of title menu, so it controls own state to avoid confusion
    public void onExitButtonPressed()
    {
        if (bootstrap_scene == null || bootstrap_script == null)
        {
            Debug.LogError("LevelSelectManager: bootstrap scene (" + bootstrap_scene + ") or script reference (" + bootstrap_script + ") not set.");
            return;
        }
        bootstrap_script.ChangeGameState_TitleMenu();
        closeMenu();
    }

    // duplicate confirm/exit logic
    private void closeMenu()
    {
        clearLevels();
        // set the menu inactive
        gameObject.SetActive(false);
    }

    // clear data to prevent duplicates, called at startup and on exit to confirm - no behaviour if already empty
    private void clearLevels()
    {
        all_levels = new LevelData[0];
        // clear the level select buttons from the container to prevent duplicates when reopening menu
        foreach (Transform child in level_button_container.transform)
        {
            if (child.gameObject != level_button_prefab)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // configData needs to have these values stored
    private void ensureLevelConfigExists(string levelName)
    {
        if (!GameManager.Main.Config.LevelBestTimes.ContainsKey(levelName))
        {
            GameManager.Main.Config.LevelBestTimes[levelName] = 0f;
        }

        if (!GameManager.Main.Config.LevelBestScores.ContainsKey(levelName))
        {
            GameManager.Main.Config.LevelBestScores[levelName] = 0;
        }

        if (!GameManager.Main.Config.LevelBestActions.ContainsKey(levelName))
        {
            GameManager.Main.Config.LevelBestActions[levelName] = 0;
        }

        if (!GameManager.Main.Config.LevelsCompleted.ContainsKey(levelName))
        {
            GameManager.Main.Config.LevelsCompleted[levelName] = false;
        }

        GameManager.Main.SaveConfig();
    }

}