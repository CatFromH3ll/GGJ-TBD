using Unity.VisualScripting;
using UnityEngine;

public class EntrenceClosingScript : MonoBehaviour
{
    public GameObject blockingBlock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            blockingBlock.SetActive(true);
        }
    }
}
