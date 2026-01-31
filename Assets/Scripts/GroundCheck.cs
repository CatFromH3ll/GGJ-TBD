using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    // Reference to the parent script
    private PlayerMovement player;

    void Start()
    {
        // Automatically find the PlayerMovement script on the parent
        player = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            player.SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            player.SetGrounded(false);
        }
    }
}