using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 6.0f;
    public float crouchScaleY = 0.5f;
    private Vector3 originalScale;  
    
    [Header("Inventory (Auto-filled by Pickups)")]
    public bool hasGravityMask = false;
    public bool hasFreezeMask = false;

    [Header("Mask Visuals (Drag child objects here)")]
    public GameObject gravityMask;
    private bool gravityMaskActive = false;
    public GameObject freezeMask;
    private bool freezeMaskActive = false;
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public bool isGrounded;
    private bool isGravityFlipped = false;
    private bool isTimeSlowed = false;
    public LayerMask groundLayer; 
    public float rayLength = 1.5f; // Used for slope rotation

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        // Ensure masks start hidden
        if (gravityMask != null) gravityMask.SetActive(false);
        if (freezeMask != null) freezeMask.SetActive(false);
        // Record the starting size of the player
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. GRAVITY MASK TOGGLE (Requires Pickup)
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasGravityMask)
        {
            gravityMaskActive = !gravityMaskActive;
            
            if (gravityMaskActive)
            {
                freezeMaskActive = false;
                if (freezeMask != null) freezeMask.SetActive(false);
                ResetTime(); 
            }
            else if (isGravityFlipped) ToggleGravity();

            if (gravityMask != null) gravityMask.SetActive(gravityMaskActive);
        }

        // 2. FREEZE MASK TOGGLE (Requires Pickup)
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasFreezeMask)
        {
            freezeMaskActive = !freezeMaskActive;

            if (freezeMaskActive)
            {
                gravityMaskActive = false;
                if (gravityMask != null) gravityMask.SetActive(false);
                if (isGravityFlipped) ToggleGravity();
            }
            else if (isTimeSlowed) ResetTime();

            if (freezeMask != null) freezeMask.SetActive(freezeMaskActive);
        }

        // --- ABILITIES ---
        if (gravityMaskActive && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleGravity();
        }

        if (freezeMaskActive && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleSlowMotion();
        }

        // --- MOVEMENT ---
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * speed, rb.linearVelocity.y);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
            isGrounded = false;
        }
        
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
        {
            transform.localScale = new Vector3(originalScale.x, crouchScaleY, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
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
        rb.gravityScale *= -1; 
        transform.eulerAngles = new Vector3(0, 0, isGravityFlipped ? 180f : 0f);
        isGrounded = false;
        if (isGravityFlipped)
        {
            sr.flipX = true;
            
        }
        else
        {
            sr.flipX = false;
        }
        
    }
}