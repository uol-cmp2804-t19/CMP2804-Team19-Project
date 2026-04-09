using UnityEngine;

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
                // owning_level.AddScore();
            }
        }
    }

    public abstract class Collectable : MonoBehaviour
    {
        public LevelMapManager owning_level = null;

        // define functionality in derived classes
        protected abstract void Collected();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Collected();
                Destroy(gameObject);
            }
        }
    }

}
