using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // public var adds export
    public Transform target;

    // processed after all updates
    void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            -10
        );
    }
}