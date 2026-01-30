using Unity.VisualScripting;
using UnityEngine;

public class GroundColl : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            playerMovement.isGrounded = true;   
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            playerMovement.isGrounded = false;   
        }
    }
}
