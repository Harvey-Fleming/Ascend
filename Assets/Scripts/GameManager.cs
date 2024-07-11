using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using LootLocker.Requests;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    int totalTime = 0;

    int leaderboardID = 22844;

    [SerializeField] private GameObject leaderboardPrefab;
    [SerializeField] private Transform leaderboardParent;

    [SerializeField] [Space] private bool shouldLoad;

    public int TotalTime { get => totalTime; set => totalTime = value; }
    public string leaderboardKey { get; private set; }

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

        leaderboardKey = "towertime";
    }

    private void Start()
    {
        StartCoroutine(LoginRoutine());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(TimeManager.instance.Timer.Millisecond + TimeManager.instance.Timer.Second * 1000 + ((TimeManager.instance.Timer.Minute * 60) * 1000) + ((TimeManager.instance.Timer.Hour * 3600) * 1000));
        }


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
            Debug.Log(blackImage.color.a);
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

    #region - Login

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player Logged in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Could not start Session");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
    #endregion

    #region - Score Section

    public void SubmitScore()
    {
        StartCoroutine(SubmitScoreRoutine());
    }

    public IEnumerator SubmitScoreRoutine()
    {
        //Convert Time to Score
        int score = TimeManager.instance.Timer.Millisecond + TimeManager.instance.Timer.Second * 1000 + ((TimeManager.instance.Timer.Minute * 60) * 1000) + ((TimeManager.instance.Timer.Hour * 3600) * 1000);

        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, score, leaderboardID.ToString(), (response) =>
        {
            if (response.success)
            {
                Debug.Log("Uploaded Score");
                done = true;
            }
            else
            {
                Debug.Log("Could not upload Score");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public void RetrieveTop10Scores()
    {
        LootLockerSDKManager.GetScoreList(leaderboardKey, 5, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                foreach(LootLockerLeaderboardMember member in response.items)
                {
                    GameObject leaderboardElement = Instantiate(leaderboardPrefab, leaderboardParent);
                    DateTime time = new();
                    Debug.Log(member.score);
                    time = time.AddMilliseconds(member.score);
                    Debug.Log("#" + member.rank + "  " + member.player.id + "      " + time.ToString("HH:mm:ss:fff"));
                    //leaderboardElement.GetComponent<TextMeshProUGUI>().text = "#" + member.rank + "  " + member.player.id + "      " + time.;

                    if (time.Hour > 0)
                    {
                        leaderboardElement.GetComponent<TextMeshProUGUI>().text = "#" + member.rank + "  " + member.player.id + "      " + time.ToString("HH:mm:ss:fff");
                    }
                    else if (time.Minute > 0)
                    {
                        leaderboardElement.GetComponent<TextMeshProUGUI>().text = "#" + member.rank + "  " + member.player.id + "      " + time.ToString("mm:ss:fff");
                    }
                    else
                    {
                        leaderboardElement.GetComponent<TextMeshProUGUI>().text = "#" + member.rank + "  " + member.player.id + "      " + time.ToString("ss:fff");
                    }

                }
            }
            else
            {
                Debug.Log("failed: " + response.errorData);
            }
        });

    }

    #endregion

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
