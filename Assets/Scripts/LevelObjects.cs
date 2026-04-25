using UnityEngine;
using System.Collections;

//TODO need to find the level without being assigned for ease of use by team
namespace LevelObjects
{

    public class ScoreCollectable : Collectable
    {
        public int score = 1;

        protected override void Collected()
        {
            if (owning_level != null && !is_collected)
            {
                is_collected = true;
                owning_level.AddScore(score);
            }
            else
            {
                Debug.LogWarning("You have not set owning_level on LevelCollectable!");
            }
        }
    }

    public abstract class Collectable : MonoBehaviour
    {
        public LevelMapManager owning_level = null;
        protected bool is_collected = false;
        protected bool destroy_on_collect = true;

        // define functionality in derived classes
        protected abstract void Collected();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (is_collected == true) { return; }
                //Debug.Log("Collected!");
                Collected();
                if (destroy_on_collect)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
