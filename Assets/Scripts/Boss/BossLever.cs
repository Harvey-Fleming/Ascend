using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLever : MonoBehaviour
{

    [SerializeField] GameObject leverPlatform;

    [SerializeField] float downTime = 1f;
    public UnityEvent leverFlipped;
    bool hasUsed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasUsed)
        {
            hasUsed = true;
            
            GetComponent<Animator>().SetTrigger("On");
            if (leverPlatform != null)
            {
                leverPlatform.SetActive(true);
            }
            StartCoroutine(ResetLeverRoutine());
        }
    }

    public void ResetLever()
    {
        StartCoroutine(ResetLeverRoutine());
    }

    IEnumerator ResetLeverRoutine()
    {
        yield return new WaitForSeconds(downTime);
        hasUsed = false;
        GetComponent<Animator>().SetTrigger("Off");
        if (leverPlatform != null)
        {
            leverPlatform.SetActive(false);
        }
    }
}
