using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

/// <summary>
/// Game loader handles the single-page application structure of the game, using a series of toggled prefabs rather than loading/unloading scenes.
/// Game loader is responsible for toggling UI state & loading/unloading levels
/// </summary>
public class main_bootstrap : MonoBehaviour
{
    // inspector set references for things existing in heirarchy, can add by scene if preferable
    public GameObject debug_level = null;
    public GameObject coding_block_parser = null;
    public GameObject coding_blocks_ui = null;
    public GameObject main_menu_reference = null;
    public GameObject pause_menu_reference = null;
    public GameObject settings_menu_reference = null;
    public GameObject main_camera = null;

    public GameObject level_1_1 = null;
    public GameObject level_1_2 = null;
    public GameObject level_1_3 = null;

    // level prefab & attached script component
    private GameObject level_instance = null;
    private LevelMapManager levelManager = null;

    // references
    private GameObject cb_parser_instance = null;
    private GameObject cb_ui_instance = null;
    private GameObject main_menu_ui_instance = null;
    private GameObject pause_menu_ui_instance = null;
    private GameObject settings_menu_ui_instance = null;



    // to fix the dynamic instantaion problems from level select
    public void startHardcodedLevel(string level_name)
    {
        if (level_name == "Level 1-1" && level_1_1 != null)
        {
            level_instance = level_1_1;
            startLevel();
        }
        else if (level_name == "Level 1-2" && level_1_2 != null)
        {
            level_instance = level_1_2;
            startLevel();
        }
        else if (level_name == "Level 1-3" && level_1_3 != null)
        {
            level_instance = level_1_3;
            startLevel();
        }
        else
        {
            Debug.Log("Could not startHardcodedLevel with arg " + level_name);
        }
    }

    // called before start
    void Awake()
    {
        LoadInstances();
        //TODO add level select to load from selection of prefabs given argument (after level design)
        // for level select need to check for relevant unloading/game controller setup
        LoadDebugLevelIfExists();
    }

    private void Start()
    {
        ChangeGameState_TitleMenu();
    }

    // before changing state to GAME_ACTIVE make sure the level instance is set
    public void ChangeGameState_ActiveGame()
    {
        if (main_menu_ui_instance != null && level_instance != null && levelManager != null)
        {
            main_menu_ui_instance.SetActive(false);
            cb_ui_instance.SetActive(true);
            TogglePause(false);
            ToggleSettings(false);
            level_instance.SetActive(true);
            GameManager.Main.current_game_state = GameManager.GAME_STATE.GAME_ACTIVE;           
            levelManager.ActivateLevel();
        }
        else
        {
            Debug.Log("Cannot change to active game state, check boostrap referneces in editor inspector!");
        }
    }


    public void ChangeGameState_TitleMenu()
    {
        if (main_menu_ui_instance != null && cb_ui_instance != null && level_instance != null && levelManager != null)
        {
            level_instance.SetActive(false);
            levelManager.DeactivateLevel();
            cb_ui_instance.SetActive(false);
            // this does not complete level or output metrics!!
            TogglePause(false);
            ToggleSettings(false);
            GameManager.Main.current_game_state = GameManager.GAME_STATE.TITLE_MENU;
            main_menu_ui_instance.SetActive(true);
        }
        else
        {
            Debug.Log("Main menu not set cannot enable!");
        }

    }

    // pass level stats 
    public void CompleteLevel()
    {
        if (level_instance == null)
        {
            Debug.Log("Set level before changing to active game state!");
            return;
        }

        if (levelManager == null)
        {
            Debug.Log("Level instance missing LevelMapManager component!");
            return;
        }

        //TODO transition state will stop gameplay execution and hotkey control
        GameManager.Main.current_game_state = GameManager.GAME_STATE.TRANSITION;
        
        //TODO open level metric screen, open and have transition between game, level metric, and title
        
        // store and log (for debug) level metrics - remove log when level metric implemented
        levelManager.saveLevelMetricsToConfig();
        Debug.Log("Level completed with score: " + levelManager.levelScore + " in time: " + levelManager.levelTimeSeconds + " and block queue size: " + levelManager.blockQueueSize);

        // when level metric screen is implemented remove this and allow the level metric to control transition to title menu
        ChangeGameState_TitleMenu();
    }

    // change gameState back to title after reporting victory (//TODO this is where to add the level metric screen)
    public void Victory()
    {
        // TODO add handling to stop CB block execution & play when in transition
        GameManager.Main.current_game_state = GameManager.GAME_STATE.TRANSITION;
        ChangeGameState_TitleMenu();
    }

    // loads a level from the given resource path
    public void LoadLevel(string resourcePath)
    {
        // unload previous level if it exists
        if (level_instance != null)
        {
            Destroy(level_instance);
        }

        // load the new level from resources
        GameObject levelPrefab = Resources.Load<GameObject>(resourcePath);
        if (levelPrefab == null)
        {
            Debug.LogError("Failed to load level from path: " + resourcePath);
            return;
        }

        level_instance = Instantiate(levelPrefab, transform);
        startLevel();
    }

    private void LoadInstances()
    {
        // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects
        // (supports both methods)

        // coding blocks and code parser
        if (coding_block_parser != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (coding_block_parser.scene.IsValid())
            {
                cb_parser_instance = coding_block_parser;
            }
            else
            {
                cb_parser_instance = Instantiate(coding_block_parser, transform);
            }
        }

        // console log, block workspace, and block palette interface
        if (coding_blocks_ui != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (coding_blocks_ui.scene.IsValid())
            {
                cb_ui_instance = coding_blocks_ui;
            }
            else
            {
                cb_ui_instance = Instantiate(coding_blocks_ui, transform);
            }
        }

        // main menu UI, pause & settings are usually already loaded in the main_boostrap
        if (main_menu_reference != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (main_menu_reference.scene.IsValid())
            {
                main_menu_ui_instance = main_menu_reference;
            }
            else
            {
                main_menu_ui_instance = Instantiate(main_menu_reference, transform);
            }
        }

        // main menu UI, pause & settings are usually already loaded in the main_boostrap
        if (settings_menu_reference != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (settings_menu_reference.scene.IsValid())
            {
                settings_menu_ui_instance = settings_menu_reference;
            }
            else
            {
                settings_menu_ui_instance = Instantiate(settings_menu_reference, transform);
            }
        }

        // main menu UI, pause & settings are usually already loaded in the main_boostrap
        if (pause_menu_reference != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (main_menu_reference.scene.IsValid())
            {
                pause_menu_ui_instance = pause_menu_reference;
            }
            else
            {
                pause_menu_ui_instance = Instantiate(pause_menu_reference, transform);
            }
        }
    }

    private LevelMapManager GetCurrentLevelManager()
    {
        if (level_instance != null)
        {
            LevelMapManager levelManager = level_instance.GetComponentInChildren<LevelMapManager>(true);
            if (levelManager != null)
            {
                return levelManager;
            }
            else
            {
                Debug.LogError("Current level instance does not have a LevelMapManager component!");
                return null;
            }
        }
        else
        {
            Debug.LogError("No current level instance found!");
            return null;
        }
    }

    // does not update game state, DRY function to use with actual game state functions
    private void ToggleSettings(bool toggle)
    {
        if (settings_menu_ui_instance != null)
        {
            settings_menu_ui_instance.SetActive(toggle);
        }
        else
        {
            Debug.Log("Settings menu not set cannot disable!");
        }
    }
    // does not update game state, DRY function to use with actual game state functions
    private void TogglePause(bool toggle)
    {
        if (pause_menu_ui_instance != null)
        {
            pause_menu_ui_instance.SetActive(toggle);
        }
        else
        {
            Debug.Log("Pause menu not set cannot disable!");
        }
    }

    // placeholder to load the testing game level, do not set debug level in live play, loading is handled by level select
   private void LoadDebugLevelIfExists()
    {
        if (debug_level != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (debug_level.scene.IsValid())
            {
                level_instance = debug_level;
                level_instance.SetActive(false);
                levelManager = GetCurrentLevelManager();
            }
            else
            {
                level_instance = Instantiate(debug_level, transform);
                level_instance.SetActive(false);
                levelManager = GetCurrentLevelManager();
            }
        }
    }

    private void startLevel()
    {
        level_instance.SetActive(true);
        levelManager = GetCurrentLevelManager();
        levelManager.InitialiseLevel();

        // setup references to level, player, and camera (code blocks/ui connection)
        CodeBlockUI codeBlockUI = cb_ui_instance.GetComponent<CodeBlockUI>();
        codeBlockUI.levelManager = levelManager;
        CBLogic cbParser = cb_parser_instance.GetComponentInChildren<CBLogic>();

        PlayerController player = levelManager.getPlayer();

        cbParser.activePlayer = player;
        player.level = levelManager;

        CameraFollow cameraFollow = main_camera.GetComponent<CameraFollow>();
        cameraFollow.target = levelManager.player.transform;

        ChangeGameState_ActiveGame();

    }

}