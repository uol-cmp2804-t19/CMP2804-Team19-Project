using UnityEngine;
using System.Collections;

//TODO need to find the level without being assigned for ease of use by team
namespace LevelObjects
{

    public class ScoreCollectable : Collectable
    {
        public int score;

        protected override void Collected()
        {
            if (owning_level != null)
            {
                //no parameter for testing just adds 1
                owning_level.AddScore();
            }
        }
    }

    public class GoalCollectable : Collectable
    {
        GoalCollectable()
        {
            destroy_on_collect = false;
        }
        protected override void Collected()
        {
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

    public abstract class Collectable : MonoBehaviour
    {
        public LevelController owning_level = null;
        protected bool is_collected = false;
        protected bool destroy_on_collect = true;

        // define functionality in derived classes
        protected abstract void Collected();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (is_collected == true) { return; }
                Collected();
                if (destroy_on_collect)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
