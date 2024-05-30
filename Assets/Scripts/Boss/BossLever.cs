using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLever : MonoBehaviour
{
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    HeartBoss hboss;
    SpriteRenderer sr;

    [SerializeField] GameObject leverPlatform;

    bool hasUsed;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        hboss = FindObjectOfType<HeartBoss>();
        sr.sprite = up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasUsed && hboss.hasStarted)
        {
            hasUsed = true;
            
            sr.sprite = down;
            if(leverPlatform != null)
            {
                leverPlatform.SetActive(true);
            }
            hboss.TakeDamage();
        }
    }

    public void Reset()
    {
        hasUsed = false;
        sr.sprite = up;
        if (leverPlatform != null)
        {
            leverPlatform.SetActive(false);
        }
    }
}
