using UnityEngine;
using UnityEngine.SceneManagement;

public class Traps : MonoBehaviour
{
    // Dropdown to select trap type in Inspector
    public enum TrapType { Static, SpinningSaw, FallingBlock }
    public TrapType trapType;
    public float rotationSpeed = 200f;
    public float detectionDistance = 10f;
    public LayerMask playerLayer;
    private Rigidbody2D rb;
    private bool hasFallen = false;
    void Start()
    {
        if (trapType == TrapType.FallingBlock)// If it is a falling block, get the Rigidbody and freeze it initially
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

    void DropBlock()
    {
        hasFallen = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable gravity
            rb.gravityScale = 2f; // Make it fall fast (optional)
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) RestartLevel();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) RestartLevel();
    }
    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}