using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class HeartBossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossBlockingPlatform;
    public bool hasStarted = false;
    public bool hasrunDialogue = false;

    [SerializeField] Cinemachine.CinemachineVirtualCamera bossCam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(!hasStarted && !hasrunDialogue)
            {
                hasrunDialogue = true;
                CameraManager.instance.ForceSwapCamera(bossCam);
                GameObject.FindObjectOfType<DialogueRunner>().StartDialogue("heartprefight");

            }
        }
    }

    [YarnCommand("StartHeartFight")]
    public void StartHeartBoss()
    {
        hasStarted = true;

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

        bossBlockingPlatform.SetActive(false);

        if (FindObjectOfType<PlayerMovement>().IsInverse)
        {
            GravityPower.StaticActivate();
        }
    }
}
