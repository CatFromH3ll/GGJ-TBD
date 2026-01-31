using UnityEngine;

public class GroundColl : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    void Update()
    {
// Ensure the circle is touching the ground layer
        bool touchingGround = Physics2D.OverlapCircle(transform.position, checkRadius, groundLayer);
    
        // Check if we are currently moving upwards (jumping)
        // This helps prevent "sticky" grounding during the start of a jump
        Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
        if (rb.linearVelocity.y > 0.1f && !playerMovement.hasGravityMask) 
        {
            touchingGround = false;
        }

        playerMovement.isGrounded = touchingGround;
    }

    // This lets you see the check circle in the editor so you can size it right
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}