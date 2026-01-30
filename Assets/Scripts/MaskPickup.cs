using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    public enum MaskType { Gravity, Freeze }
    [Header("Select which mask this item is:")]
    public MaskType maskToGive;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure the object touching the mask is tagged "Player"
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                if (maskToGive == MaskType.Gravity) player.hasGravityMask = true;
                else if (maskToGive == MaskType.Freeze) player.hasFreezeMask = true;

                Debug.Log(maskToGive + " Mask added to inventory!");
                
                // Destroy the mask on the floor after picking it up
                Destroy(gameObject);
            }
        }
    }
}