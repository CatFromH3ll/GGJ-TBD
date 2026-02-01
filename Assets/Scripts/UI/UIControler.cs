using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;   
    public GameObject startMenu;   
    private bool isPaused = true;


    private void Awake()
    {
        startMenu.SetActive(true);
        Time.timeScale = 0f; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }


    public void Quit()
    {
        Application.Quit();
    }
     
    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void Startmenu()
    {
       isPaused = false;

        startMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}