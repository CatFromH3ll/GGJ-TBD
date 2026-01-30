using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    
    [Header("Masks")]
    public GameObject gravityMask;
    private bool gravityMaskActive = false; // Tracks if the mask is "on"
    public GameObject freezeMask;
    private bool freezeMaskActive = false;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isGravityFlipped = false;
    private bool isTimeSlowed = false;
    
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gravityMask != null) gravityMask.SetActive(false); //set the mask off by default
        if (freezeMask != null) freezeMask.SetActive(false); //set the mask off by the default
    }

    void Update()
    {
        // 1. MASK TOGGLE (Press 1 to put the mask on/off)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gravityMaskActive = !gravityMaskActive;
            
            // IF TURNING ON GRAVITY: Turn off Freeze Mask automatically
            if (gravityMaskActive)
            {
                freezeMaskActive = false;
                if (freezeMask != null) freezeMask.SetActive(false);
                ResetTime(); // Always reset time if switching masks
            }
            else if (isGravityFlipped)
            {
                ToggleGravity();
            }

            if (gravityMask != null) gravityMask.SetActive(gravityMaskActive);
        }
        // 2.MASK TOGGLE (press 2 to put the mask on/off)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            freezeMaskActive = !freezeMaskActive;

            // IF TURNING ON FREEZE: Turn off Gravity Mask automatically
            if (freezeMaskActive)
            {
                gravityMaskActive = false;
                if (gravityMask != null) gravityMask.SetActive(false);
            }

            if (isGravityFlipped)
            {
                ToggleGravity();
            }
            
            else if (isTimeSlowed)
            {
                ResetTime(); // Reset time if you take the mask off
            }

            if (freezeMask != null) freezeMask.SetActive(freezeMaskActive);
        }

        // 2. GRAVITY ABILITY (Press G only if mask is on and we are grounded)
        if (gravityMaskActive && isGrounded && Input.GetKeyDown(KeyCode.G))
        {
            ToggleGravity();
        }

        if (freezeMaskActive && Input.GetKeyDown(KeyCode.F))
        {
            ToggleSlowMotion();
        }

        // 3. MOVEMENT
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * speed, rb.linearVelocity.y);

        // 4. JUMPING (Added parentheses to fix the logic)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
    
    void ToggleSlowMotion()
    {
        isTimeSlowed = !isTimeSlowed;
        Traps.GlobalTimeFactor = isTimeSlowed ? 0.5f : 1.0f;
    }

    void ResetTime()
    {
        isTimeSlowed = false;
        Traps.GlobalTimeFactor = 1.0f;
    }
    
    void ToggleGravity()
    {
        isGravityFlipped = !isGravityFlipped;
        rb.gravityScale *= -1; // Flip gravity

        // Flip the player visual
        float rotationZ = isGravityFlipped ? 180f : 0f;
        transform.eulerAngles = new Vector3(0, 0, rotationZ);
        
        isGrounded = false; // Player is now "falling" to the new floor
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}