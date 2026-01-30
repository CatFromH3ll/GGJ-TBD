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
    public bool isGrounded;
    private bool isGravityFlipped = false;
    private bool isTimeSlowed = false;

    public SpriteRenderer sr;

    // --- NEW: SLOPE DETECTION VARIABLES ---
    public LayerMask groundLayer; // Set this to "Ground" in the Inspector
    public float rayLength = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gravityMask != null) gravityMask.SetActive(false);
        if (freezeMask != null) freezeMask.SetActive(false);
        originalScale = transform.localScale;

        // Keep rotation frozen from physics, but we will control it via code
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. GRAVITY MASK TOGGLE
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

        // 2. FREEZE MASK TOGGLE
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

        // --- NEW: FACE LOOK LEFT/RIGHT ---
        if (xInput > 0) sr.flipX = false;
        else if (xInput < 0) sr.flipX = true;

        // --- NEW: SLOPE ALIGNMENT LOGIC ---
        // Shoot a ray downwards (or upwards if gravity is flipped)
        Vector2 rayDir = isGravityFlipped ? transform.up : -transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, rayLength, groundLayer);

        if (hit.collider != null)
        {
            // Calculate the angle of the ground
            float angle = Vector2.SignedAngle(isGravityFlipped ? Vector2.down : Vector2.up, hit.normal);
            // Apply the rotation so the player "leans" into the slope
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            // Reset to flat if in the air
            transform.eulerAngles = new Vector3(0, 0, isGravityFlipped ? 180f : 0f);
        }

        // --- JUMP & CROUCH ---
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
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
    }
    
}