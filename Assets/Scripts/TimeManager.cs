using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance { get; private set; }


    DateTime timer;
    [SerializeField] TMP_Text timerText;

    [SerializeField] Button startButton;

    bool isRunning = false;

        public DateTime Timer { get => timer; set => timer = value; }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            timer = timer.AddSeconds(1 * Time.deltaTime);

            if (timer.Hour > 0)
            {
                timerText.text = timer.ToString("HH:mm:ss:fff");
            }
            else if (timer.Minute > 0)
            {
                timerText.text = timer.ToString("mm:ss:fff");
            }
            else
            {
                timerText.text = timer.ToString("ss:fff");
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
        return timer;
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
