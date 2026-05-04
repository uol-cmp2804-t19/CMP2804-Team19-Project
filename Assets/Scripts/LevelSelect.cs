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
        // for loading level_instance in main_bootstrap, add to scene and set inactive then pass ref to main
        public string resourcePath;
        // name displays on button, other metrics on info panel (time stored as processed ticks, needs to be converted for display)
        public string levelName;
        public int bestScore;
        public float bestTime;
        public int bestQueueSize;
    }

    // bootstrap is the main game scene controller handling prefab loading
    public main_bootstrap main = null;

    // following references must be set in editor
    // -    container is the left-most field container buttons
    // -    prefab is the testing button prefab, should be set inactive, will be duplicated to make level buttons
    // -        (this button requires child with text component)
    // -    info panel refs are for text labels in the right-most field displaying level metrics
    //          (these text labels will be updated when level button clicked)

    public GameObject level_button_container = null;
    public GameObject level_button_prefab = null;
    // info panel text labels
    public GameObject level_info_score = null;
    public GameObject level_info_time = null;
    public GameObject level_info_queue = null;

    // scene buttons need to be connected to onStartButtonPressed & onExitButtonPressed respectively
    public GameObject button_start_level = null;
    public GameObject button_exit_menu = null;

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

    public void openMenu()
    {
        // populate level select buttons from Resources/GameLevels
        // -    for each prefab, load level name from config and display on button
        // -    for each prefab, load level metrics from config and display on info panel on button hover
        // -    add listener to button to reference file when clicked and pass prefab to game manager
        // -    add listener to button to display level metrics on info panel when clicked

        // load levels, set buttons, update metric display to initial option
        populateLevelsFromDisk();
        buildLevelSelectButtons();
        updateLevelInfo();
        gameObject.SetActive(true);
    }

    //TODO whenever button 
    private void updateLevelInfo() {
        if (level_info_score != null) { level_info_score.GetComponent<Text>().text = "Best Score: " + active_level_selected.bestScore.ToString(); }
        if (level_info_time != null) { level_info_time.GetComponent<Text>().text = "Best Time: " + active_level_selected.bestTime.ToString("F2") + "s"; }
        if (level_info_queue != null) { level_info_queue.GetComponent<Text>().text = "Best Queue Size: " + active_level_selected.bestQueueSize.ToString(); }
    }

    //TODO move to Config or utility? any library to support this?
    //convert game ticks for level record to HH:MM:SS
    private string convertToTimeStamp(float ticks)
    {
        int seconds = (int)(ticks % 60);
        int minutes = (int)((ticks / 60) % 60);
        int hours = (int)((ticks / 3600) % 24);
        string output = "";

        output = (seconds % 60).ToString("00");
        output = ((minutes % 60)).ToString("00") + ":" + output;
        output = (hours).ToString("00") + ":" + output;

        return output;
    }

    private void buildLevelSelectButtons()
    {
        // create buttons with listeners for each level, duplicating from the prefab
        if (level_button_container == null || level_button_prefab == null)
        {
            Debug.LogError("LevelSelectManager: level button container and/or prefab not set in inspector!");
            return;
        }
        else {
            foreach (LevelData levelData in all_levels)
            {
                // clone the debug button as child of button container
                GameObject button = Instantiate(level_button_prefab, level_button_container.transform);

                // confirm structure, edit this code if changes are made
                Text buttonText = button.GetComponentInChildren<Text>();
                Button buttonComponent = button.GetComponent<Button>();
                
                if (buttonText == null || buttonComponent == null)
                {
                    Debug.LogError("Level button prefab is missing Text and/or Button component.");
                    Destroy(button);
                    continue;
                }

                // button name (displayed as text) will be level name, clicking will update info panel/selected level
                buttonText.text = levelData.levelName;
                buttonComponent.onClick.AddListener(() => {
                    active_level_selected = levelData;
                    updateLevelInfo();
                });
                
                // debug button is set inactive, toggle
                button.SetActive(true);
            }
        }
    }

    // levels currently exist in ./Assets/Prefabs/GameLevels/
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
            
            // set active level to top of list by default, can change when button clicked
            if (i == 0) { active_level_selected = levelData; }
            // resize array to add new level data, add to array
            System.Array.Resize(ref all_levels, all_levels.Length + 1);
            all_levels[all_levels.Length - 1] = levelData;
        }

    }

    // connect start button to this
    // when level is selected (start pressed) need to tell bootstrap to actually load the level prefab (wait for it) & close this menu
    // bootstrap will handle transition to game state as this behaviour is owned by that
    public void onStartButtonPressed() {
        // check if valid path
        if (string.IsNullOrEmpty(active_level_selected.resourcePath))
        {
            Debug.LogError("No level selected!");
            return;
        }
        // else
        // pass the file path of the chosen level
        if (main == null)
        {
            Debug.LogError("LevelSelectManager: main bootstrap reference not set.");
            return;
        }
        main.LoadLevel(active_level_selected.resourcePath);
        closeMenu();
    }

    // connect exit button to this
    // levelSelect is not part of game state it is a submenu of title menu, so it controls own state to avoid confusion
    public void onExitButtonPressed() {
        main.ChangeGameState_TitleMenu();
        closeMenu();
    }

    // duplicate confirm/exit logic
    private void closeMenu()
    {
        clearLevels();
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

}