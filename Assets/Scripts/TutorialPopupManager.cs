using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupManager : MonoBehaviour // Inherit from MonoBehaviour
{
    public GameObject TutorialPopup;

    public KeyCode OpenPopupKey = KeyCode.F1;

    private bool isPopupOpen = false;

    private void Start()
    {
        isPopupOpen = TutorialPopup.activeSelf;
    }

    void Update()
    {
        if (Input.GetKeyDown(OpenPopupKey))
        {
            if (isPopupOpen)
            {
                isPopupOpen = false;
                TutorialPopup.SetActive(false);
            }
            else
            {
                isPopupOpen = true;
                TutorialPopup.SetActive(true);
            }
        }
    }
}
