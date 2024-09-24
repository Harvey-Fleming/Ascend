using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
   
    private float level = 0;

    [SerializeField] [Space] private bool shouldLoad;
    [SerializeField] private bool isHardMode;

    public bool IsHardMode { get => isHardMode; set => isHardMode = value; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

    }

    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings )
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void StartFadeToBlack()
    {
        StartCoroutine(FadeToBlack(1));
    }


    public void StartFadeFromBlack()
    {
        StartCoroutine(FadeFromBlack(1));
    }

    [YarnCommand("FadeToBlack")]
    public static IEnumerator FadeToBlack(int lerpSpeed)
    {
        float t = 0f;
        Image blackImage = GameObject.Find("Black Screen Image").GetComponent<Image>();
        while (blackImage.color.a <= 0.9)
        {
            t += lerpSpeed *Time.deltaTime;

            blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.Lerp(blackImage.color.a, 1, t));
            //Debug.Log(blackImage.color.a);
            yield return new WaitForSeconds(0.1f);
        }
        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, 1);
        yield return null;
    }

    [YarnCommand("FadeFromBlack")]
    public static IEnumerator FadeFromBlack(int lerpSpeed)
    {
        float t = 0f;
        Image blackImage = GameObject.Find("Black Screen Image").GetComponent<Image>();
        while (blackImage.color.a >= 0.1)
        {
            t += lerpSpeed * Time.deltaTime;
            blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.Lerp(blackImage.color.a, 0, t));
            yield return new WaitForSeconds(0.1f);
        }
        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, 0);
        yield return null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGame();
        level = scene.buildIndex;
        switch(level)
        {
            case 0:
                AudioManager.instance.Play("MainMenuMusic");
                break;            
            case 1:
                AudioManager.instance.Play("VillageMusic");
                break;            
            case 2:
                AudioManager.instance.Play("MountainMusic");
                break;
            case 3:
                AudioManager.instance.Play("TowerMusic");
                break;
            case 4:
                AudioManager.instance.Play("SkyMusic");
                break;

        }
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    #region - Saving
    public void SaveGame()
    {
        GlobalEvents.OnSaveData();
    }

    private void LoadGame()
    {
        if(shouldLoad)
        {
            GlobalEvents.OnLoadData();
        }
    }

    public void NewGame()
    {
        SaveSystem.ClearSaveData();
    }

    
    #endregion
}
