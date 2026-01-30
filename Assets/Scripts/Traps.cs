using UnityEngine;
using UnityEngine.SceneManagement;

public class Traps : MonoBehaviour
{
    // Dropdown to select trap type in Inspector
    public float rotationSpeed = 200f;
    public float detectionDistance = 10f; 
    public enum TrapType { Static, SpinningSaw, FallingBlock, Spear }
    public TrapType trapType;
    public LayerMask playerLayer;
    
    public GameObject TrapGround;
    private Rigidbody2D rb;
    
    private bool hasFallen = false;
    void Start()
    {
        if (trapType == TrapType.FallingBlock || trapType == TrapType.Spear)// If it is a falling block OR SPEAR, get the Rigidbody and freeze it initially
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic; // Floating in air
            }
            else
            {
                Debug.LogError("Falling Block needs a Rigidbody2D!");
            }
        }
    }
    void Update()
    {
        if (trapType == TrapType.SpinningSaw) // Handle Spinning Saw Logic
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (trapType == TrapType.FallingBlock && !hasFallen) // Handle Falling Block Logic
        {
            DetectPlayerBelow();
        }
        if (trapType == TrapType.Spear && !hasFallen) // Handle Spear Logic
        {
            DetectPlayerAbove();
        }
    }
    void DetectPlayerBelow()
    {
        // Cast a ray straight down to look for the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionDistance, playerLayer);

        // Draw a line in the Scene view so you can see the detection range (Debug only)
        Debug.DrawRay(transform.position, Vector2.down * detectionDistance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            DropBlock();
        }
    }

    void DetectPlayerAbove()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, detectionDistance, playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ShootSpear();
        }
    }

    void DropBlock()
    {
        hasFallen = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable gravity
            rb.gravityScale = 2f; // Make it fall fast (optional)
        }
    }
    void ShootSpear()
    {
        hasFallen = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable gravity
            rb.gravityScale = -2f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) RestartLevel();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (trapType == TrapType.FallingBlock) // --- FALLING BLOCK LOGIC ---
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f) RestartLevel(); // Normal points UP = Player is BELOW the block (Crushed)
                }
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                rb.bodyType = RigidbodyType2D.Static; // Freeze on floor
                TrapGround.SetActive(true);
            }
        }
        else if (trapType == TrapType.Spear) // --- SPEAR LOGIC (New) ---
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f) RestartLevel(); // Normal points DOWN = Player is ABOVE the spear (Impaled)
                }
            }
            else if (collision.gameObject.CompareTag("Ground")) // Stick to the Ceiling
            {
                rb.bodyType = RigidbodyType2D.Static; // Freeze on ceiling
            }
        }
        else // --- OTHER TRAPS (Spikes/Saws) ---
        {
            if (collision.gameObject.CompareTag("Player")) RestartLevel(); // Instantly kill on any touch
        }
    }
    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}