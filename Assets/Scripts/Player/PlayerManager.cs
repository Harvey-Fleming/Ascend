using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainMenu;

    [SerializeField] bool isPaused = false;

    public bool IsPaused { get => isPaused;}

    private void Start()
    {
        if(mainMenu.activeInHierarchy)
        {
            isPaused = true;
            Time.timeScale = 0;
        }
        else
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = true;
            TimeManager.instance.StartTimer();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }

        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void StopPlayerMovement()
    {
        PlayerEvents.OnMovementActive(false);
    }

    public void ResumePlayerMovement()
    {
        PlayerEvents.OnMovementActive(true);
    }
}
