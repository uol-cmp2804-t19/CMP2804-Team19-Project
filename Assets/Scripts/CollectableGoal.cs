using UnityEngine;
using System.Collections;
using LevelObjects;

//TODO need to find the level without being assigned for ease of use by team
namespace GoalCollectable
{

    public class GoalCollectable : Collectable
    {
        public main_bootstrap GameManager = null;
        GoalCollectable()
        {
            //Debug.Log("setup goal collectable");
            destroy_on_collect = false;
        }
        protected override void Collected()
        {
            Debug.Log("GAME OVER!");
            if (GameManager != null && !is_collected)
            {
                is_collected = true;
                GameManager.CompleteLevel();
                //TODO destroy level on return to title?
            }
            else
            {
                Debug.LogWarning("You have not set GameManager on LevelGoal!");
            }
        }
    }

}
