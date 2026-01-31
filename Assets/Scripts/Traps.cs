using UnityEngine;
using UnityEngine.SceneManagement;

public class Traps : MonoBehaviour
{
    public enum TrapType { Static, SpinningSaw, FallingBlock, Spear, Axe }
    public TrapType trapType;
    public LayerMask playerLayer;

    public bool isDangerousWhenFalling = true;
    private bool hasTriggered = false;
    private bool isStuck = false; 
    public bool throwToLeft = false;
    public bool artFacesLeft = false;

    public float rotationSpeed = -300f;
    public float detectionDistance = 20f;
    public float throwSpeed = 30f;
    public static float GlobalTimeFactor = 1.0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        if (trapType == TrapType.FallingBlock || trapType == TrapType.Spear || trapType == TrapType.Axe)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic; // Freeze initially
        }
        
        // Ensure Falling Block is treated as Ground so you can jump on it before AND after it falls
        if (trapType == TrapType.FallingBlock)
        {
            gameObject.tag = "Ground";
        }

        if (trapType == TrapType.Axe)
        {
            sr = GetComponent<SpriteRenderer>();
            bool shouldFlip = throwToLeft;
            if (artFacesLeft) shouldFlip = !shouldFlip;
            if (sr != null) sr.flipX = shouldFlip;
        }
    }

    void Update()
    {
        if (trapType == TrapType.SpinningSaw)
        {
            transform.Rotate(0, 0, rotationSpeed * GlobalTimeFactor * Time.deltaTime);
        }
        else if (trapType == TrapType.Axe)
        {
            if (hasTriggered && !isStuck)
            {
                float spinDirection = throwToLeft ? -1f : 1f;
                transform.Rotate(0, 0, rotationSpeed * spinDirection * GlobalTimeFactor * Time.deltaTime);
            }
            else if (!hasTriggered)
            {
                Vector2 dir = throwToLeft ? Vector2.left : Vector2.right;
                DetectPlayerInDirection(dir);
            }
        }
        else if (!hasTriggered) 
        {
            if (trapType == TrapType.FallingBlock) DetectPlayerInDirection(Vector2.down);
            else if (trapType == TrapType.Spear) DetectPlayerInDirection(Vector2.up);
        }
    }

    void DetectPlayerInDirection(Vector2 direction)
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, playerLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position , direction, detectionDistance, playerLayer);
        Debug.DrawRay(transform.position, direction * detectionDistance, Color.red);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ActivateTrap(direction);
        }
    }

    void ActivateTrap(Vector2 dir)
    {
        hasTriggered = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            if (trapType == TrapType.FallingBlock) rb.gravityScale = 2f;
            else if (trapType == TrapType.Spear) rb.gravityScale = -2f;
            else if (trapType == TrapType.Axe)
            {
                rb.gravityScale = 0f; 
                rb.linearVelocity = dir * throwSpeed;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (trapType == TrapType.Axe) // 1. AXE LOGIC =========================================================================
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (!isStuck) RestartLevel(); 
                else
                {
                    if (collision.otherCollider.CompareTag("Ground")) return; 
                    RestartLevel();
                }
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                FreezeTrap();
            }
        }
        else if (trapType == TrapType.FallingBlock) // 2. FALLING BLOCK LOGIC ================================================
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (isStuck) return; // If the block is stuck (landed), it's just a normal wall/floor, We return immediately so no damage logic runs.

                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f)
                    {
                        return;
                    }
                    if (hasTriggered)
                    {
                        RestartLevel();
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Ground")) // If it hits the floor, it stops and becomes safe terrain
            {
                FreezeTrap();
            }
        }
        else if (trapType == TrapType.Spear) // 3. SPEAR LOGIC ==================================================================
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f) RestartLevel();
                }
            }
            else if (collision.gameObject.CompareTag("Ground")) FreezeTrap();
        }
        else // 4. STATIC TRAPS (SPIKES) ========================================================================================
        {
            if (collision.gameObject.CompareTag("Player")) RestartLevel();
        }
    }
    void FreezeTrap()
    {
        isStuck = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector2.zero;
        }
        if (trapType != TrapType.Axe) // Ensure it is tagged Ground so PlayerMovement sees it as jumpable
        {
            gameObject.tag = "Ground";
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) RestartLevel();
    }
    void RestartLevel()
    {
        GlobalTimeFactor = 1.0f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}