using UnityEngine;

public class GroundColl : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float yOffset;
    
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    void Update()
    {
        var colliderPos = playerMovement.IsGravityFlipped ? -yOffset : yOffset;
        transform.localPosition = new Vector3(transform.localPosition.x, colliderPos, 0);

        bool touchingGround = Physics2D.OverlapCircle(transform.position, checkRadius, groundLayer);
        playerMovement.isGrounded = touchingGround;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}