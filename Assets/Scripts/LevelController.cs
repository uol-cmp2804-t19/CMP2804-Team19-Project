using UnityEngine;

// TODO
// Level receives input from coding block tells player to move based on grid

public class LevelController : MonoBehaviour
{
    public PlayerController player = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    // Update behaviour is just for testing whilst code blocks aren't integrated
    void Update()
    {
        // testing behaviour only
        // TODO remove
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        // Vector2 dir = (x, y);

        if (x > 0.0)
        {
            Debug.Log("Move Right");
        }
        else if (y > 0.0)
        {
            Debug.Log("Move Up");
        }
        else if (x < 0.0)
        {
            Debug.Log("Move Left");
        }
        else if (y < 0.0)
        {
            Debug.Log("Move Down");
        }

        // Vector3 movement = new Vector3(x, y, 0);
        // transform.position += movement * speed * Time.deltaTime;

    }

    void MovePLayer()
    {
        if (player != null or )
        {
            //TODO add error handling
            Debug.Log("You forgot to assign a player!");
            return;
        }

    }
}
