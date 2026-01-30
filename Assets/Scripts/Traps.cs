using UnityEngine;
using UnityEngine.SceneManagement;

public class Traps : MonoBehaviour
{
    public enum TrapType { Static, SpinningSaw, FallingBlock, Spear, Axe }
    public TrapType trapType;

    [Header("Settings")]
    public float rotationSpeed = 300f;
    public float detectionDistance = 20f; 
    public float throwSpeed = 30f;
    public static float GlobalTimeFactor = 1.0f; 
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private bool hasTriggered = false; // Replaced "hasFallen" for clarity
    private bool isStuck = false;      // New: tracks if axe hit the wall

    void Start()
    {
        if (trapType == TrapType.FallingBlock || trapType == TrapType.Spear || trapType == TrapType.Axe)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic; // Freeze initially
        }
    }

    void Update()
    {
        if (trapType == TrapType.SpinningSaw) // SPINNING SAW (Always spins)
        {
            transform.Rotate(0, 0, rotationSpeed * GlobalTimeFactor * Time.deltaTime);
        }
        else if (trapType == TrapType.Axe) // THROWING AXE (Spins only when flying)
        {
            if (hasTriggered && !isStuck) // If it has been thrown BUT hasn't hit the wall yet -> SPIN
            {
                transform.Rotate(0, 0, rotationSpeed * GlobalTimeFactor * Time.deltaTime);
            }
            // If it hasn't been thrown yet -> LOOK FOR PLAYER
            else if (!hasTriggered)
            {
                // transform.right automatically handles rotation (Left or Right)
                DetectPlayerInDirection(transform.right);
            }
        }
        else if (!hasTriggered) // OTHER TRAPS (Detection)
        {
            if (trapType == TrapType.FallingBlock) DetectPlayerInDirection(Vector2.down);
            else if (trapType == TrapType.Spear) DetectPlayerInDirection(Vector2.up);
        }
    }

    void DetectPlayerInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, playerLayer);
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
                rb.gravityScale = 0f; // Fly straight
                rb.linearVelocity = dir * throwSpeed; // Fly in the direction we looked
            }
        }
    }

    // --- COLLISION LOGIC ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. AXE LOGIC
        if (trapType == TrapType.Axe)
        {
            // If it hits the player...
            if (collision.gameObject.CompareTag("Player"))
            {
                // If it is flying (dangerous) -> Restart
                if (!isStuck) RestartLevel();
                // If it is stuck (platform) -> Do nothing (safe to walk on)
            }
            // If it hits the wall/ground -> Stick
            else if (collision.gameObject.CompareTag("Ground"))
            {
                FreezeTrap();
            }
        }

        // 2. FALLING BLOCK LOGIC
        else if (trapType == TrapType.FallingBlock)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f) RestartLevel(); // Crushed
                }
            }
            else if (collision.gameObject.CompareTag("Ground")) FreezeTrap();
        }

        // 3. SPEAR LOGIC
        else if (trapType == TrapType.Spear)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f) RestartLevel(); // Impaled
                }
            }
            else if (collision.gameObject.CompareTag("Ground")) FreezeTrap();
        }

        // 4. GENERIC TRAPS
        else 
        {
            if (collision.gameObject.CompareTag("Player")) RestartLevel();
        }
    }

    void FreezeTrap()
    {
        isStuck = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static; // Stop moving
            rb.linearVelocity = Vector2.zero;     // Stop physics velocity
        }
        gameObject.tag = "Ground"; // Become walkable
        
        // Optional: Re-align rotation to look nice on the wall?
        // transform.rotation = Quaternion.Euler(0, 0, 0); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) RestartLevel();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}