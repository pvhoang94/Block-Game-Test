using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }
}
