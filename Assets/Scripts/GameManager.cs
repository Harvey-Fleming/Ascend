using System.Collections;
using System;
using UnityEngine;
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

    public void StartFadeToBlack()
    {
        Image blackImage = GameObject.Find("Black Screen Image").GetComponent<Image>();

        if(blackImage != null)
        {
            StartCoroutine(FadeToBlack(blackImage));

        }
    }

    IEnumerator FadeToBlack(Image blackImage)
    {
        float t = 0f;
        while (blackImage.color.a <= 0.9)
        {
            t += 1 * Time.deltaTime;
            blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.Lerp(blackImage.color.a, 1, t));
            yield return new WaitForSeconds(0.1f);
        }
        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, 1);
        FindObjectOfType<FinalScreen>().OnShowScreen(FindObjectOfType<TimeManager>().GetTimer(), FindObjectOfType<CollectCoin>().CoinsGathered);
        yield return null;
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
}
