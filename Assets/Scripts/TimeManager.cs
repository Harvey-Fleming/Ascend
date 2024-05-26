using System;
using System.Collections;
using UnityEngine;
using TMPro;


public class TimeManager : MonoBehaviour
{
    DateTime time;
    [SerializeField] TMP_Text timerText;



    // Update is called once per frame
    void Update()
    {
        time = time.AddSeconds(1 * Time.deltaTime);

        if(time.Hour > 0)
        {
            timerText.text = time.ToString("HH:mm:ss:fff");
        }
        else if(time.Minute > 0)
        {
            timerText.text = time.ToString("mm:ss:fff");
        }
        else
        {
            timerText.text = time.ToString("ss:fff");
        }
    }



    
}
