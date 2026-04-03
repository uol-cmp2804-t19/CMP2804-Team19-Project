using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace CBClass
{
    public class CodeBlock
    {
        private GameObject container;
        private List<string> action_list = new List<string>();
        public int orderNumber;
        private bool type_loop;
        public bool HasSpace = true;

        /// <summary>
        /// main constructor for CBClass objects
        /// </summary>
        /// <param name="parent_container"> the parent UnityAsset </param>
        /// <param name="order"> the order it is in the Block UI</param>
        /// <param name="loop"> if it is a looping block</param>
        public CodeBlock(GameObject parent_container, int order, bool loop, CBLogic controller)
        {
            container = parent_container;
            type_loop = loop;
            orderNumber = order;
            if (loop) { container.tag = "ActionLoop"; }
            else { container.tag = "ActionSingle"; }

            //cursed stuff right here, pls don't judge <3
            // adds a onclick listener call to the RemoveActionBlock via lambda expression
            // this is done to add the ordernumber to the function call on the newly created game objects
            container.GetComponent<Button>().onClick.AddListener(() => controller.removeActionBlock(orderNumber));
        }

        /// <summary>
        /// Assigns an action to the code block (adds to the end of the list if loop is enabled)
        /// </summary>
        /// <param name="action"> the action to add as a string </param>
        public void AssignAction(string action)
        {
            if (type_loop)
            {
                action_list.Add(action);
            }
            else
            {
                action_list.Clear();
                action_list.Add(action);
                HasSpace = false;
            }
        }

        /// <summary>
        /// ends the loop block input if it is loop block
        /// </summary>
        public void EndLoop()
        {
            if (type_loop)
            {
                HasSpace = false;
            }
        }

        /// <summary>
        /// Gets the Array of actions stored in the class
        /// </summary>
        /// <returns> The Array of Actions </returns>
        public List<string> GetActions()
        {
            return action_list;
        }

        public void emptyBlock()
        {
            GameObject.Destroy(container);
        }

    }
}
