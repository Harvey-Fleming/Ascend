using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossBlockingPlatform;
    public bool hasStarted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(!hasStarted)
            {
                hasStarted = true;
                Camera.main.orthographicSize = 10;
                Vector3 camlocalPos = Camera.main.transform.localPosition;
                camlocalPos.y += 5;
                Camera.main.transform.localPosition = camlocalPos;

                HeartBoss hboss = FindObjectOfType<HeartBoss>();
                hboss.StartFight();

                bossBlockingPlatform.SetActive(true);

                if(FindObjectOfType<PlayerMovement>().IsInverse)
                {
                    GravityPower.StaticActivate();
                }
                
            }
        }
    }

    public void Reset()
    {
        hasStarted = false;
        Camera.main.orthographicSize = 7.5f;
        Vector3 camlocalPos = Camera.main.transform.localPosition;
        camlocalPos.y -= 5;
        Camera.main.transform.localPosition = camlocalPos;

        bossBlockingPlatform.SetActive(false);

        if (FindObjectOfType<PlayerMovement>().IsInverse)
        {
            GravityPower.StaticActivate();
        }
    }
}
