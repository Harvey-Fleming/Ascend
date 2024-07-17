using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] [Space] private bool shouldLoad;

    private void Awake()
    {
        
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

    }

    private void Start()
    {
        AudioManager.instance.Play("VillageMusic");
    }

    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings )
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartFadeToBlack()
    {
        StartCoroutine(FadeToBlack(1));
    }


    public void StartFadeFromBlack()
    {
        StartCoroutine(FadeFromBlack(1));
    }

    [Yarn.Unity.YarnCommand("FadeToBlack")]
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

    [Yarn.Unity.YarnCommand("FadeFromBlack")]
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
