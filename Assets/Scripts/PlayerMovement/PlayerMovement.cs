using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float waitToRotateSpeed0=0.2f;
    public float speed = 5.0f;
    public float jumpForce = 6.0f;
    public float crouchScaleY = 0.5f;
    private Vector3 originalScale;  

    [Header("Inventory")]
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
    public bool IsGravityFlipped => isGravityFlipped;

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
        if (gravityMaskActive && isGrounded && Input.GetKeyDown(KeyCode.Space))
            ToggleGravity();

        if (freezeMaskActive && Input.GetKeyDown(KeyCode.Space))
            ToggleSlowMotion();

        // --- MOVEMENT ---
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * speed, rb.linearVelocity.y);

        if (xInput > 0) sr.flipX = false;
        else if (xInput < 0) sr.flipX = true;

        // --- JUMP & CROUCH ---
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
        }
        
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
            transform.localScale = new Vector3(originalScale.x, originalScale.y * crouchScaleY, originalScale.z);
        else
            transform.localScale = originalScale;
    }
    
    void FixedUpdate()
    {
        Vector2 rayDir = isGravityFlipped ? transform.up : -transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, rayLength, groundLayer);

        /*if (isGrounded)
        {
            //float angle = Vector2.SignedAngle(isGravityFlipped ? Vector2.up : Vector2.down, hit.normal);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {*/
            float targetZ = isGravityFlipped ? 180f : 0f;
            transform.rotation = Quaternion.Euler(targetZ, 0, 0);
        
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
        StartCoroutine(TurnOffGroundDetectionForFewFrames());
        isGravityFlipped = !isGravityFlipped;

        // flip gravity
        rb.gravityScale *= -1;

        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // clear Y velocity
        //rb.AddForce((isGravityFlipped ? Vector2.up : Vector2.down) * 2f, ForceMode2D.Impulse);

        isGrounded = false;
        sr.flipY = isGravityFlipped;
    }

    private IEnumerator TurnOffGroundDetectionForFewFrames()
    {
        var groundCheck = GetComponentInChildren< GroundColl > ();
        var OwnCollision = GetComponent<Collider2D>();
        groundCheck.enabled = false;
        OwnCollision.enabled = false;
        
        yield return new WaitForSeconds(waitToRotateSpeed0);
        OwnCollision.enabled = true;
        groundCheck.enabled = true;
        
    }
}