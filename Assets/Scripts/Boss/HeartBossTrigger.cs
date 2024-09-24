using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class HeartBossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossBlockingPlatform;
    public bool hasStarted = false;
    public bool hasrunDialogue = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(!hasStarted && !hasrunDialogue)
            {
                hasrunDialogue = true;
                GameObject.FindObjectOfType<DialogueRunner>().StartDialogue("heartprefight");

            }
        }
    }

    [YarnCommand("StartHeartFight")]
    public void StartHeartBoss()
    {
        hasStarted = true;
        Camera.main.orthographicSize = 10;

        HeartBoss hboss = FindObjectOfType<HeartBoss>();
        hboss.StartFight();

        bossBlockingPlatform.SetActive(true);

        if (FindObjectOfType<PlayerMovement>().IsInverse)
        {
            GravityPower.StaticActivate();
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
