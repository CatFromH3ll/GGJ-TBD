using UnityEngine;

public class victoryScript : MonoBehaviour
{
    public GameObject victoryScrean;
    bool isPaused;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.tag == "Player")
        {
            victoryScrean.SetActive(true);
            Time.timeScale = 0f;
        }

    }

}
