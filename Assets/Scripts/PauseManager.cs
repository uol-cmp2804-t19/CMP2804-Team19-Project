using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	public GameObject pauseMenuUI;
	public GameObject bootstrap_scene;
	private main_bootstrap bootstrap_script;
	private bool isPaused = false;
    public GameObject settingsPanel;

	void start()
	{
		getBootstrapScriptComponent();
	}

	void Update()
	{
		// Currently using the Escape key to toggle pause - cannot do whilst animating or on main menu

		if (GameManager.Main.current_game_state == GameManager.GAME_STATE.TITLE_MENU || GameManager.Main.current_game_state == GameManager.GAME_STATE.TRANSITION )
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
		//SceneManager.LoadScene("MainMenu");
		bootstrap_script.ChangeGameState_TitleMenu();
	}
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
}
