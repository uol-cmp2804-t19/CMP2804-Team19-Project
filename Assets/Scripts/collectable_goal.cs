using UnityEngine;
using System.Collections;
using LevelObjects;

//TODO need to find the level without being assigned for ease of use by team
namespace GoalCollectable
{

    public class GoalCollectable : Collectable
    {
        GoalCollectable()
        {
            destroy_on_collect = false;
        }
        protected override void Collected()
        {
            Debug.Log("Collected!");
            if (owning_level != null)
            {
                Victory();
            }
        }
        IEnumerator Victory()
        {
            yield return new WaitForSeconds(3);
            Application.Quit();
        }
    }

}
