using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Follow the position
            transform.position = target.position + offset;

            // 2. FORCE the rotation to stay upright (Fixes the flip)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}