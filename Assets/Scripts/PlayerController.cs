using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 30.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    // placeholder testing behaviour
    //void Update()
    //{
    //    float x = Input.GetAxisRaw("Horizontal");
    //    float y = Input.GetAxisRaw("Vertical");

    //    Vector3 movement = new Vector3(x, y, 0);
    //    transform.position += movement * speed * Time.deltaTime;
    //}

    public void Move(Vector2 dir)
    {
        Vector3 movement = new Vector3(dir.x, dir.y, 0);
        transform.position += movement * speed * Time.deltaTime;
    }
}
