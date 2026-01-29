using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables visible in the Unity Inspector
    public float speed = 4.0f;       // How fast the player moves left/right
    public float jumpForce = 6.0f;   // The strength of the upward "kick"
    
    // Internal variables
    private Rigidbody2D rb;          // Reference to the Physics component
    private bool isGrounded;         // A "toggle" to know if we are on the floor

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Link our 'rb' variable to the actual Rigidbody2D component on this object
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once every frame (handling Input)
    void Update()
    {
        // 1. HORIZONTAL MOVEMENT
        // Get input from A/D or Left/Right arrows (-1 to 1)
        float xInput = Input.GetAxis("Horizontal");
        
        // Create a Vector3 to store our target velocity
        // x = input * speed
        // y = we keep the current vertical velocity (so gravity still works!)
        // z = 0 (since it's a 2D game)
        Vector3 moveDirection = new Vector3(xInput * speed, rb.linearVelocity.y, 0f);
        
        // Set the physics velocity to our new Vector3
        // Note: Unity automatically converts Vector3 to Vector2 here by ignoring Z
        rb.linearVelocity = moveDirection;

        // 2. JUMPING
        // Check if the "Jump" key is pressed (Space) AND the player is on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Vector3.up is shorthand for (0, 1, 0)
            // ForceMode2D.Impulse makes the force happen instantly (perfect for jumps)
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            
            // Set grounded to false so we can't jump again while in mid-air
            isGrounded = false;
        }
    }

    // This function runs automatically when the player's collider hits another collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object we bumped into has the tag "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If it does, we are back on the floor and can jump again
            isGrounded = true;
        }
    }
}