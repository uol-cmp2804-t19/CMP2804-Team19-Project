using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public main_bootstrap main_game;
	public GameObject settingsPanel;
	public int totalLevels;


    public GameObject levelSelectPrefab;
	private LevelSelectManager levelSelectScript;

    private void Start()
    {
        GetLevelSelectComponent();
    }

    public void OpenSettings()
	{
		if (settingsPanel == null)
		{
			Debug.Log("settings menu not assigned in inspector!");
			return;
		}

        Debug.Log("Opening settings menu");
        settingsPanel.SetActive(true);
        //GameManager.Main.current_game_state = GameManager.GAME_STATE.SETTINGS_MENU_FROM_TITLE;
    }

	public void CloseSettings()
    {
        if (settingsPanel == null)
        {
            Debug.Log("settings menu not assigned in inspector!");
            return;
        }
        Debug.Log("Closing settings menu");
        settingsPanel.SetActive(false);
        //GameManager.Main.current_game_state = GameManager.GAME_STATE.TITLE_MENU;
    }

    private void GetLevelSelectComponent()
    {
        if (levelSelectPrefab != null)
        {
            levelSelectScript = levelSelectPrefab.GetComponent<LevelSelectManager>();
        }
        else
        {
            Debug.Log("levelSelectPrefab not set in editor!");
        }
    }

    // defunct function
    /*
	public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
	}
	*/

    // defunct function
    // no longer in use, play game converted to use level select
    /*
    public void LoadLevelSelect()
    {
        Debug.Log("Implement the start game functionality! Pass to LevelManager tracking last level");
        //SceneManager.LoadScene("LevelSelect");
	}
	*/

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // play game now level select
    public void PlayGame()
    {
        if (levelSelectPrefab != null || levelSelectScript != null)
        {
            levelSelectScript.openMenu();
        }

        //Debug.Log("Implement the start game functionality! Pass to LevelManager tracking last level");

		//to review 'play next level functionality' as looking at just having level select
		/*
		int highestCompletedLevel = 0;

		if (GameManager.Main != null && GameManager.Main.Config != null)
		{
			highestCompletedLevel = GameManager.Main.Config.LevelsCompleted;
		}
		else
		{
			Debug.LogWarning("GameManager or config not initialized. Defaulting to Level 1.");
		}

		int nextLevel = highestCompletedLevel + 1;

		if (nextLevel > totalLevels)
		{
			nextLevel = totalLevels;
		}

		SceneManager.LoadScene("Level" + nextLevel);
		*/
	}
}
