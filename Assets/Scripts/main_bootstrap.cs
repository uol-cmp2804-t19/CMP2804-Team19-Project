using UnityEngine;

public class main_bootstrap : MonoBehaviour
{
    public GameObject debug_level = null;
    public GameObject coding_block_parser = null;
    public GameObject coding_blocks_ui = null;

    private GameObject level_instance = null;
    private GameObject cb_parser_instance = null;
    private GameObject cb_ui_instance = null;

    // called before start
    void Awake()
    {
        ///
        LoadCodingBlocks();
        LoadLevel();

    }
    
    void LoadCodingBlocks()
    {
        // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects
        // (supports both methods)

        // coding blocks and code parser
        if (coding_block_parser != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (coding_block_parser.scene.IsValid())
            {
                cb_parser_instance = coding_block_parser;
            }
            else
            {
                cb_parser_instance = Instantiate(coding_block_parser, transform);
            }
        }

        // console log, block workspace, and block palette interface
        if (coding_blocks_ui != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (coding_blocks_ui.scene.IsValid())
            {
                cb_ui_instance = coding_blocks_ui;
            }
            else
            {
                cb_ui_instance = Instantiate(coding_blocks_ui, transform);
            }
        }

    }

    //TODO modify to load from selection of prefabs given argument (after level design) and check for relevanting unloading/game controller setup
    void LoadLevel()
    {
        if (debug_level != null)
        {
            // ignore prefab instantiation if dragging from hierarchy view prefab-as-scene-objects

            if (debug_level.scene.IsValid())
            {
                level_instance = debug_level;
            }
            else
            {
                level_instance = Instantiate(debug_level, transform);
            }
        }
    }

}
