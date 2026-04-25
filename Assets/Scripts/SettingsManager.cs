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

	public void CloseSettings()
	{
		gameObject.SetActive(false);
	}
}
