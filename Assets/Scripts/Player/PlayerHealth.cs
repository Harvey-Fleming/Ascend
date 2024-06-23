using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int health = 1;
    int maxHealth = 1;

    GameObject lastCheckpoint;

    [SerializeField] GameObject defaultCheckpoint;

    [SerializeField] Sprite checkpointSprite1;
    [SerializeField] Sprite checkpointSprite2;

    public UnityEvent playerDeathEvent;

    float[] savedPos;

    public GameObject LastCheckpoint { get => lastCheckpoint; set => lastCheckpoint = value; }
    public GameObject DefaultCheckpoint { get => defaultCheckpoint; set => defaultCheckpoint = value; }

    private void Awake()
    {
        lastCheckpoint = defaultCheckpoint;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Heart")
        {
            TakeDamage();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint")
        {
            lastCheckpoint.GetComponent<SpriteRenderer>().sprite = checkpointSprite1;
            lastCheckpoint = collision.gameObject;
            lastCheckpoint.GetComponent<SpriteRenderer>().sprite = checkpointSprite2;
            GameManager.instance.SaveGame();
        }
        else if (collision.gameObject.tag == "DeathBox")
        {
            PlayerDeath();
        }
    }

    public void TakeDamage()
    {
        health--;

        if(health <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("Player has died");
        playerDeathEvent.Invoke();
        Respawn();
    }

    private void Respawn()
    {
        health = maxHealth;
        if(lastCheckpoint == null)
        {
            lastCheckpoint = defaultCheckpoint;
        }
        transform.position = lastCheckpoint.transform.position;

        if(GetComponent<PlayerMovement>().IsInverse)
        {
            GravityPower.StaticActivate();
        }
    }

    public void LoadData(object sender, SaveEventArg saveArg)
    {
        //Load player next to last checkpoint
        savedPos = new float[3];
        savedPos = saveArg.playerData.lastCheckpointPos;
        transform.position = new Vector3(savedPos[0], savedPos[1], savedPos[2]);
    }

    private void OnEnable()
    {
        GlobalEvents.LoadData += LoadData;
    }

    private void OnDisable()
    {
        GlobalEvents.LoadData -= LoadData;
    }
}
