using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource uiSpeaker;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    void Start()
    {
        // If the speaker slot is empty, search the scene for an AudioSource automatically!
        if (uiSpeaker == null)
        {
            uiSpeaker = FindFirstObjectByType<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiSpeaker != null && hoverSound != null)
        {
            uiSpeaker.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiSpeaker != null && clickSound != null)
        {
            uiSpeaker.PlayOneShot(clickSound);
        }
    }
}