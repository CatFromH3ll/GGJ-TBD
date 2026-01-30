using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;

    void Start() => effector = GetComponent<PlatformEffector2D>();

    void Update()
    {
        // When S is held, flip the effector so the player falls through
        if (Input.GetKey(KeyCode.S)) {
            effector.rotationalOffset = 180f;
        } else {
            effector.rotationalOffset = 0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            effector.rotationalOffset = 180f;
        }
        
    }
}
