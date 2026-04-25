using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public GameObject settingsPanel;
	public int totalLevels;

	public void OpenSettings()
	{
		settingsPanel.SetActive(true);
	}

	public void CloseSettings()
	{
		settingsPanel.SetActive(false);
	}

	public void LoadLevel(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	// no longer in use, play game converted to use level select
	public void LoadLevelSelect()
    {
        Debug.Log("Implement the start game functionality! Pass to LevelManager tracking last level");
        SceneManager.LoadScene("LevelSelect");
	}

	// play game now level select
	public void PlayGame()
	{
		Debug.Log("Implement the start game functionality! Pass to LevelManager tracking last level");

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
