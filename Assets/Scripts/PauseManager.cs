using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	public GameObject pauseMenuUI;
	private bool isPaused = false;
    public GameObject settingsPanel;

	void Update()
	{
        // Currently using the Escape key to toggle pause

		if (GameManager.Main.current_game_state == GameManager.GAME_STATE.TITLE_MENU || GameManager.Main.current_game_state == GameManager.GAME_STATE.SETTINGS_MENU_FROM_TITLE)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isPaused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	public void ResumeGame()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
	}

	public void PauseGame()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		isPaused = true;
	}

	public void LoadMainMenu()
	{
		Time.timeScale = 1f;
		Debug.Log("TODO - You need to revert to main menu by pressing this, tell GameLoader to unload active level etc.");
		//SceneManager.LoadScene("MainMenu");
	}
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
}
