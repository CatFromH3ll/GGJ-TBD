using UnityEngine;

public class RotateToPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float rayLength = 1.0f;
    public LayerMask groundLayer;
    public float rotationSpeed = 15f;

    // References the script on the parent
    private PlayerMovement playerMovement;

    void Awake()
    {
        // --- NEW: LOOK AT PARENT FOR SCRIPT ---
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        RotateToGround();
    }

    void RotateToGround()
    {
        bool isFlipped = false;

        if (playerMovement != null)
        {
            // Note: Ensure 'isGravityFlipped' is PUBLIC in your PlayerMovement script
            // isFlipped = playerMovement.isGravityFlipped; 
        }

        // Shoot ray relative to the child's current orientation
        Vector2 rayDir = isFlipped ? transform.up : -transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, rayLength, groundLayer);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            // Calculate ground angle using the hit normal
            float targetAngle = Vector2.SignedAngle(Vector2.up, hit.normal);

            // Apply to rotation
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Return to base rotation (0 or 180)
            float resetAngle = isFlipped ? 180f : 0f;
            Quaternion resetRot = Quaternion.Euler(0, 0, resetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, resetRot, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Direction is 'down' relative to the object for the editor preview
        Vector3 direction = -transform.up; 
        Gizmos.DrawRay(transform.position, direction * rayLength);
        Gizmos.DrawWireCube(transform.position + (direction * rayLength), new Vector3(0.1f, 0.1f, 0.1f));
    }
}