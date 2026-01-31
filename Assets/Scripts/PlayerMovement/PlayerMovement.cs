using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float waitToRotateSpeed0 = 0.2f;
    public float speed = 5.0f;
    public float jumpForce = 6.0f;
    public float crouchScaleY = 1f;
    private Vector3 originalScale;

    [Header("Inventory")] public bool hasGravityMask = false;
    public bool hasFreezeMask = false;

    [Header("Mask Visuals")] public GameObject gravityMask;
    private bool gravityMaskActive = false;
    public GameObject freezeMask;
    private bool freezeMaskActive = false;

    public GameObject gravitySprite;
    public GameObject freezeSprite;

    public Animator anim;
    [SerializeField] private RuntimeAnimatorController NoMask;
    [SerializeField] private RuntimeAnimatorController GravityMask;
    [SerializeField] private RuntimeAnimatorController FreezeMask;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public bool isGrounded;
    private bool isGravityFlipped = false;
    public bool IsGravityFlipped => isGravityFlipped;

    private bool isTimeSlowed = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (gravityMask != null) gravityMask.SetActive(false);
        if (freezeMask != null) freezeMask.SetActive(false);
        originalScale = transform.localScale;
        anim.SetBool("nomask", true);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        GameObject spaawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        if (spaawnPoint != null)
        {
            transform.position = spaawnPoint.transform.position;
        }
    }

    void Update()
    {
        // --- MASK TOGGLES ---
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasGravityMask)
        {
            gravityMaskActive = !gravityMaskActive;
            if (gravityMaskActive)
            {
                anim.SetBool("nomask", false);
                anim.SetBool("gravityMask", true);
                anim.SetBool("freezeMask", false);
                StartCoroutine(ChangeMask(GravityMask));
                freezeMaskActive = false;
                if (freezeMask != null)
                {
                    freezeMask.SetActive(false);
                    ResetTime();
                }
            }
            else
            {
                anim.SetBool("nomask", true);
                anim.SetBool("gravityMask", false);
                anim.SetBool("freezeMask", false);
                StartCoroutine(ChangeMask(NoMask));
            }

            // else if (isGravityFlipped) ToggleGravity();
            if (gravityMask != null)
            {
                gravityMask.SetActive(gravityMaskActive);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && hasFreezeMask)
        {
            freezeMaskActive = !freezeMaskActive;
            if (freezeMaskActive)
            {
                anim.SetBool("nomask", false);
                anim.SetBool("gravityMask", false);
                anim.SetBool("freezeMask", true);
                StartCoroutine(ChangeMask(FreezeMask));
                gravityMaskActive = false;
                if (gravityMask != null)
                {
                    gravityMask.SetActive(false);
                }

                if (isGravityFlipped)
                {
                    ToggleGravity();
                }
            }
            else if (isTimeSlowed)
            {
                ResetTime();
                anim.SetBool("nomask", true);
                anim.SetBool("gravityMask", false);
                anim.SetBool("freezeMask", false);
                StartCoroutine(ChangeMask(NoMask));
            }
            else
            {
                anim.SetBool("nomask", true);
                anim.SetBool("gravityMask", false);
                anim.SetBool("freezeMask", false);
                StartCoroutine(ChangeMask(NoMask));
            }

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

        if (xInput > 0)
        {
            anim.SetBool("walk", true);
            sr.flipX = isGravityFlipped;
        }

        else if (xInput < 0)
        {
            anim.SetBool("walk", true);
            sr.flipX = !isGravityFlipped;
        }

        else
        {
            anim.SetBool("walk", false);
        }

        // --- JUMP & CROUCH ---
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            anim.SetTrigger("jump");
            float jumpDirection = isGravityFlipped ? -1f : 1f;
            rb.AddForce(Vector2.up * jumpForce * jumpDirection, ForceMode2D.Impulse);
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
        {
            anim.SetBool("crouch", true);
            //transform.localScale = new Vector3(originalScale.x, originalScale.y , originalScale.z);
        }
        //else
        //{
        //    //transform.localScale = originalScale;
        //}

        if ((Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) && isGrounded))
        {
            anim.SetBool("crouch", false);
        }
    }

    IEnumerator ChangeMask(RuntimeAnimatorController mask)
    {
        anim.SetTrigger("changeMask");
        string name_ = "";
        if (mask == NoMask) name_ = "Normal";
        else if (mask == FreezeMask) name_ = "Freeze";
        else name_ = "Gravity";
        anim.Play(name_);
        yield return new WaitForSeconds(0.3f);
        anim.Play("Idle");
        anim.runtimeAnimatorController = mask;
    }

    void FixedUpdate()
    {
        float targetZ = isGravityFlipped ? 180f : 0f;
        transform.rotation = Quaternion.Euler(0, 0, targetZ);
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
    }

    private IEnumerator TurnOffGroundDetectionForFewFrames()
    {
        var groundCheck = GetComponentInChildren<GroundColl>();
        var OwnCollision = GetComponent<Collider2D>();
        groundCheck.enabled = false;
        OwnCollision.enabled = false;
        yield return new WaitForSeconds(waitToRotateSpeed0);
        OwnCollision.enabled = true;
        groundCheck.enabled = true;
    }


    private void ToggleSprite(int spriteIndex)
    {
        // switch (spriteIndex)
        // {
        //     case 1:
        //     {
        //         GetComponent<SpriteRenderer>().enabled = true;
        //         
        //         
        //     }
        //    
        //         
        // }
    }
}