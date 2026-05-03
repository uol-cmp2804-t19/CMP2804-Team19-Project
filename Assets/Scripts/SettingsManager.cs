using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
	public AudioMixer mainMixer;

	public void SetVolume(float volume)
	{
		float dbVolume = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
		mainMixer.SetFloat("MasterVolume", dbVolume);
	}

	public void SetMute(bool isMuted)
	{
		AudioListener.pause = isMuted;
	}

	// this method must be called to save user setting preferences
	public void CloseSettings()
	{
		gameObject.SetActive(false);
		// update config on exit to prevent unnecessary file writes when adjusting settings
		gamemanager.Main.Config.setting_volume = volume;
		gamemanager.Main.Config.setting_isMuted = isMuted;
		gamemanager.Main.SaveConfig();
	}
}
