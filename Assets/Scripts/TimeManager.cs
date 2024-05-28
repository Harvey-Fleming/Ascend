using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TimeManager : MonoBehaviour
{
    DateTime time;
    [SerializeField] TMP_Text timerText;

    [SerializeField] Button startButton;

    bool isRunning = false;



    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            time = time.AddSeconds(1 * Time.deltaTime);

            if (time.Hour > 0)
            {
                timerText.text = time.ToString("HH:mm:ss:fff");
            }
            else if (time.Minute > 0)
            {
                timerText.text = time.ToString("mm:ss:fff");
            }
            else
            {
                timerText.text = time.ToString("ss:fff");
            }
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(PlayerPrefs.GetInt("Menu") == 1)
        {
            startButton.onClick.Invoke();
            PlayerPrefs.SetInt("Menu", 0);
            PlayerPrefs.Save();
        }
    }

    public void StartTImer()
    {
        isRunning = true;
    }

    public DateTime GetTimer()
    {
        return time;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}
