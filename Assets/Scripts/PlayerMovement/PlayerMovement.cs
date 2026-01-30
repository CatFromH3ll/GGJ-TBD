using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    
    [Header("Inventory (Auto-filled by Pickups)")]
    public bool hasGravityMask = false;
    public bool hasFreezeMask = false;

    [Header("Mask Visuals (Drag child objects here)")]
    public GameObject gravityMask;
    private bool gravityMaskActive = false;
    public GameObject freezeMask;
    private bool freezeMaskActive = false;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isGravityFlipped = false;
    private bool isTimeSlowed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure masks start hidden
        if (gravityMask != null) gravityMask.SetActive(false);
        if (freezeMask != null) freezeMask.SetActive(false);
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
        if (gravityMaskActive && isGrounded && Input.GetKeyDown(KeyCode.G))
        {
            ToggleGravity();
        }

        if (freezeMaskActive && Input.GetKeyDown(KeyCode.F))
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
        else if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    
}