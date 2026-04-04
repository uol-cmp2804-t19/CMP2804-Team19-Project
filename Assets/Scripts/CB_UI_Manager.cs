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

    private void Awake()
    {
        if (codeBlockLogic == null)
        {
            //TODO are we handling enough LogError validation?
            Debug.LogError("CodeBlockUI: CBLogic is not assigned in Inspector. Please find within the scene.");
        }
        // disable interface before shown, does not default to open
        mainInterfacePanel.SetActive(!mainInterfacePanel.activeSelf);
    }

    private void Start()
    {
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
            // place button with click function adding the same action type to queue
            Button blockButton = CreateBlockButton(actionType.ToString());
            blockButton.onClick.AddListener(() => QueueAdd(actionType));
            blockButton.transform.SetParent(paletteContainer);
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

        // after destroying old items, loop through the refernce to active blocks and build entirely new block UI objects
        foreach (var action in actionQueue)
        {
            Button blockButton = CreateBlockButton(action.ToString());
            blockButton.onClick.AddListener(() => QueueRemove(action));
            blockButton.transform.SetParent(queueContainer);
        }
    }

    // counterpart to QueueRemove
    private void QueueAdd(CBLogic.CBActionTypes actionType)
    {
        actionQueue.Add(actionType);
        UpdateQueueDisplay();
    }

    // counterpart to QueueAdd
    private void QueueRemove(CBLogic.CBActionTypes actionType)
    {
        actionQueue.Remove(actionType);
        UpdateQueueDisplay();
    }

    // not yet implemented, need to confirm CBUI update & integration with @Masa before integrating with world
    private void ExecuteQueue()
    {
        Debug.Log("This button runs the code blocks but this isn't implemented yet!");

        /* (pseudo)
        foreach (var action in actionQueue)
        {
            codeBlockLogic.CBAction(action);
        }
        */
    }

    // create button object & set fixed size
    private Button CreateBlockButton(string label)
    {
        Button button = new GameObject(label).AddComponent<Button>();
        button.GetComponent<RectTransform>().sizeDelta = queueButtonSize;
        return button;
    }
}