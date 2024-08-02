using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLever : MonoBehaviour
{
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    SpriteRenderer sr;

    [SerializeField] GameObject leverPlatform;

    [SerializeField] float downTime = 1f;
    public UnityEvent leverFlipped;
    bool hasUsed;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasUsed)
        {
            hasUsed = true;
            
            sr.sprite = down;
            if(leverPlatform != null)
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
        sr.sprite = up;
        if (leverPlatform != null)
        {
            leverPlatform.SetActive(false);
        }
    }
}
