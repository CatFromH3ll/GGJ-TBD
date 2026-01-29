using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables visible in the Unity Inspector
    public float speed = 4.0f;       // player speed
    public float jumpForce = 6.0f;   // player jump force
    
    public GameObject GravityMask;
    private bool isGravityFlipped = false;
    
    // Internal variables
    private Rigidbody2D rb;    // rb stands for rigidbody 
    private bool isGrounded;   // A "toggle" to know if we are on the floor

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();// get the component of the rigidbody
        
        if (GravityMask != null) //if the mask not used mean the gravity is normal
        {
            GravityMask.SetActive(false);
        }
    }

    // Update is called once every frame (handling Input)
    void Update()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown("1"))//first mask input
            {
                ToggleGravity();
                isGrounded = false;
            }
            else
            {
                //Debug.Log("you can use the mask only when you are on the ground");
            }
        }
 
        float xInput = Input.GetAxis("Horizontal");
        Vector3 moveDirection = new Vector3(xInput * speed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = moveDirection;

        // 2. JUMPING
        if (Input.GetKeyDown("w" ) || Input.GetKeyDown("up") && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            // Vector3.up is shorthand for (0, 1, 0)
            // ForceMode2D.Impulse makes the force happen instantly (perfect for jumps)
            rb.AddForce(Vector3.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
            // Set grounded to false so we can't jump again while in mid-air
            isGrounded = false;
        }
    }

    void ToggleGravity()
    {
        isGravityFlipped = !isGravityFlipped;
        rb.gravityScale = rb.gravityScale * -1;
        if (isGravityFlipped)
        {
            transform.eulerAngles = new Vector3(0, 0, 180f);
            if (GravityMask != null)
            {
                GravityMask.SetActive(true);
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0f);
            if (GravityMask != null)
            {
                GravityMask.SetActive(false);
            }
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