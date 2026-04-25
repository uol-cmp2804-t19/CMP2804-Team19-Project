using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodeBlockUI : MonoBehaviour
{
    //TODO fixed size for queue button, may not scale well with UI, to be re-evaluated
    public Vector2 queueButtonSize = new Vector2(100, 50);

    // references to UI objects
    [SerializeField] private Button buttonToggleInterface;
    [SerializeField] private GameObject mainInterfacePanel;
    [SerializeField] private Transform paletteContainer;
    [SerializeField] private Transform queueContainer;
    [SerializeField] private Button buttonPlay;
    // find on cb_prefab/GameController
    [SerializeField] private CBLogic codeBlockLogic;

    // hold a list of every action currently in the queue, build queue from this - whenever this updates call UpdateQueueDisplay()
    private List<CBLogic.CBActionTypes> actionQueue = new List<CBLogic.CBActionTypes>();
    private int maxQueueSize = 60; // visual limit to prevent overflow
    public int softMaxQueueSize = 60;

    private void Awake()
    {
        // disable interface before shown, does not default to open
        mainInterfacePanel.SetActive(!mainInterfacePanel.activeSelf);
    }

    private void Start()
    {
        if (codeBlockLogic == null)
        {
            //TODO are we handling enough LogError validation?
            Debug.Log("CodeBlockUI: CBLogic is not assigned in Inspector.");
            try
            {
                codeBlockLogic = GameObject.FindWithTag("GameController").GetComponent<CBLogic>();
                if (codeBlockLogic == null) { Debug.LogError("CodeBlockUI: CBLogic could not be assigned automatically"); }
                else { Debug.Log("CodeBlockUI: CBLogic automatically assigned"); }
            }
            catch
            {
                Debug.LogError("CodeBlockUI: CBLogic could not be assigned automatically");
            }
        }
        // open menu button calls method to flip active state of interface panel when clicked
        buttonToggleInterface.onClick.AddListener(ToggleInterface);
        // play button calls method to run the code within the level
        buttonPlay.onClick.AddListener(ExecuteQueue);
        // setup
        PopulatePalette();
    }

    private void ToggleInterface()
    {
        //Debug.Log("toggle interface clicked!!");
        mainInterfacePanel.SetActive(!mainInterfacePanel.activeSelf);
    }

    // adds block buttons for each block type listed in CBActionTypes
    private void PopulatePalette()
    {
        ///* placeholder logic to review and test
        // loop through array of the CBActionTypes enum, using the name as the button text
        foreach (CBLogic.CBActionTypes actionType in System.Enum.GetValues(typeof(CBLogic.CBActionTypes)))
        {
            Debug.LogFormat("add {0} to palette!", actionType);
            // place button with click function adding the same action type to queue
            Button blockButton = CreateBlockButton(actionType.ToString(), paletteContainer);
            blockButton.onClick.AddListener(() => QueueAdd(actionType));
        }
        //*/
    }


    // performance concerns should be minimal due to limited process competition in the game loop,
    // however, (TODO) it may be worthwhile to introduce a click delay to the button
    private void UpdateQueueDisplay()
    {
        // list is immutable - whenever the list updates, completely destroy and rebuild it
        foreach (Transform child in queueContainer)
        {
            Destroy(child.gameObject);
        }

        // after destroying old items, loop through through and build entirely new block UI objects
        for (int i = 0; i < actionQueue.Count; i++)
        {
            // if not using indexes the actionQueue will not respect the order of blocks when removing blocks
            var actionAtPos = actionQueue[i];
            int indexToRemove = i;

            Button blockButton = CreateBlockButton(actionAtPos.ToString(), queueContainer);
            blockButton.onClick.AddListener(() => QueueRemove(indexToRemove));
        }
     }

    // counterpart to QueueRemove
    private void QueueAdd(CBLogic.CBActionTypes actionType)
    {
        if (actionQueue.Count >= softMaxQueueSize && actionQueue.Count >= maxQueueSize)
        {
            //TODO - add user feedback for trying to add blocks when queue is full
            Debug.LogWarning("Queue full! TODO - add user feedback!");
            return;
        }
        actionQueue.Add(actionType);
        UpdateQueueDisplay();
    }

    // counterpart to QueueAdd
    private void QueueRemove(int index)
    {
        // Check to prevent out-of-bounds errors
        if (index >= 0 && index < actionQueue.Count)
        {
            actionQueue.RemoveAt(index);
            UpdateQueueDisplay();
        }
        else 
        {
            Debug.LogWarning("Invalid index for QueueRemove call, at idx: " + index);
        }
    }

    // not yet implemented, need to confirm CBUI update & integration with @Masa before integrating with world
    private void ExecuteQueue()
    {
        Debug.Log("This button runs the code blocks but this isn't implemented yet!");

        foreach (CBLogic.CBActionTypes action in actionQueue)
        {
            Debug.Log("actiontype "+action+" was called");
            codeBlockLogic.CBAction(action);
        }
    }

    // create button object & set fixed size
    //TODO would this be easier with a prefab instantation and property update rather than manually building in code?
    private Button CreateBlockButton(string label, Transform parent)
    {
        /*
         TODO
        CreateBlockButton parent setting can produce incorrect local scale/anchoring under UI layout groups.
        Use SetParent(parent, false) so RectTransform local values are preserved.
         */

        GameObject buttonObj = new GameObject(label);
        // set bounding box
        RectTransform button_visual_container = buttonObj.AddComponent<RectTransform>();
        button_visual_container.sizeDelta = queueButtonSize;
        // Default unity UI sprit
        Image button_visual = buttonObj.AddComponent<Image>();
        // add actual button - this is the returned object because different methods set different listeners
        Button button = buttonObj.AddComponent<Button>();
        // set hierarchy
        buttonObj.transform.SetParent(parent);

        // add hover & click colours
        button.transition = Selectable.Transition.ColorTint;
        button.targetGraphic = button_visual;
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = Color.white;                 // Default color
        buttonColors.highlightedColor = new Color(0.9f, 0.9f, 0.9f); // Slightly darker when hovered
        buttonColors.pressedColor = new Color(0.7f, 0.7f, 0.7f);     // Darker when clicked
        buttonColors.selectedColor = Color.white;
        buttonColors.colorMultiplier = 1f;
        button.colors = buttonColors;

        // TODO - text is placeholder for actual block visuals - blocks need to be assigned to an image text by type
        // add text to button
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = label;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;
        // stretch text to button
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return button;

    }
}