using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] TMP_Text counterText;

    int coinsGathered = 0;

    int leaderboardID = 22427;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(SubmitScoreRoutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            coinsGathered++;
            Destroy(collision.gameObject);
            counterText.text = coinsGathered.ToString();
        }
    }

    public IEnumerator SubmitScoreRoutine()
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, coinsGathered, leaderboardID.ToString(), (response) =>
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
}
