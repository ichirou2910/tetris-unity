using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        this.SuscribeEvent(EventID.OnGameOver, (param) => GameOver());
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Replay()
    {
        this.PublishEvent(EventID.OnReplay);
        Time.timeScale = 1f;
    }
    
    private void GameOver()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}
