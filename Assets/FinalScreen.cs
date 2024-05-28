using System;
using UnityEngine;
using TMPro;

public class FinalScreen : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text coinText;

    public void OnShowScreen(DateTime timer, float coinsCollected)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        timerText.text = "You Finished in: " + timer.ToString("HH:mm:ss:fff");
        coinText.text = "You collected " + coinsCollected + " number of coins";
    }
}
