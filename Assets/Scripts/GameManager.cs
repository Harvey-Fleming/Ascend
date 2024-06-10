using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using LootLocker.Requests;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    int totalTime = 0;

    int leaderboardID = 22427;

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
        Image blackImage = GameObject.Find("Black Screen Image").GetComponent<Image>();

        if (blackImage != null)
        {
            StartCoroutine(SubmitScoreRoutine());

        }
    }

    public IEnumerator SubmitScoreRoutine()
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, totalTime, leaderboardID.ToString(), (response) =>
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
                    leaderboardElement.GetComponent<TextMeshProUGUI>().text = "#" + member.rank + "  " + member.player.id + "      " + member.score;
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
