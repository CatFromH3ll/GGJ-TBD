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

    [Header("Mask Visuals")]
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
    public float rayLength = 1.5f; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (gravityMask != null) gravityMask.SetActive(false);
        if (freezeMask != null) freezeMask.SetActive(false);
        originalScale = transform.localScale;
        
        // This is important: stop physics from spinning the player
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // --- MASK TOGGLES ---
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasGravityMask)
        {
            gravityMaskActive = !gravityMaskActive;
            if (gravityMaskActive) { freezeMaskActive = false; if (freezeMask != null) freezeMask.SetActive(false); ResetTime(); }
            else if (isGravityFlipped) ToggleGravity();
            if (gravityMask != null) gravityMask.SetActive(gravityMaskActive);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && hasFreezeMask)
        {
            freezeMaskActive = !freezeMaskActive;
            if (freezeMaskActive) { gravityMaskActive = false; if (gravityMask != null) gravityMask.SetActive(false); if (isGravityFlipped) ToggleGravity(); }
            else if (isTimeSlowed) ResetTime();
            if (freezeMask != null) freezeMask.SetActive(freezeMaskActive);
        }

        // --- ABILITIES ---
        if (gravityMaskActive && isGrounded && Input.GetKeyDown(KeyCode.Space)) ToggleGravity();
        if (freezeMaskActive && Input.GetKeyDown(KeyCode.Space)) ToggleSlowMotion();

        // --- MOVEMENT ---
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * speed, rb.linearVelocity.y);

        // --- JUMP & CROUCH ---
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
        }
        
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
            transform.localScale = new Vector3(originalScale.x, crouchScaleY, originalScale.z);
        else
            transform.localScale = originalScale;
    }
    
    void FixedUpdate()
    {
        // This ray points DOWN when normal, and UP when gravity is flipped
        Vector2 rayDir = isGravityFlipped ? transform.up : -transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, rayLength, groundLayer);

        // ONLY align to slope if we are actually grounded
        if (hit.collider != null && isGrounded)
        {
            float angle = Vector2.SignedAngle(isGravityFlipped ? Vector2.down : Vector2.up, hit.normal);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // If in air, just stay upright (0) or upside down (180)
            float targetZ = isGravityFlipped ? 180f : 0f;
            transform.rotation = Quaternion.Euler(0, 0, targetZ);
        }
    }
    
    void ToggleSlowMotion() { isTimeSlowed = !isTimeSlowed; Traps.GlobalTimeFactor = isTimeSlowed ? 0.5f : 1.0f; }
    void ResetTime() { isTimeSlowed = false; Traps.GlobalTimeFactor = 1.0f; }
    
    void ToggleGravity()
    {
        isGravityFlipped = !isGravityFlipped;
        rb.gravityScale *= -1; 
        isGrounded = false;
        sr.flipY = isGravityFlipped;
    }
}