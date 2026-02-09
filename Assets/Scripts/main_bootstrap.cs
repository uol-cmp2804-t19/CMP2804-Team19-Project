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
        if (debug_level != null)
        {
            level_instance = Instantiate(debug_level, transform);
        }
        if (coding_block_parser != null)
        {
            // transform is parent (main_boostrap)
            cb_parser_instance = Instantiate(coding_block_parser, transform);
        }
        if (coding_blocks_ui != null)
        {
            cb_ui_instance = Instantiate(coding_blocks_ui, transform);
        }
    }

}
