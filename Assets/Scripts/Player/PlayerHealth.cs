using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    int health = 1;
    int maxHealth = 1;

    bool hasTakenDamage = false;
    [SerializeField] private bool canLoseLives = true;

    int lives = 3;
    [SerializeField] private TMP_Text liveText;

    GameObject lastCheckpoint;

    [SerializeField] GameObject defaultCheckpoint;

    [SerializeField] Sprite checkpointSprite1;
    [SerializeField] Sprite checkpointSprite2;

    float[] savedPos;

    public GameObject LastCheckpoint { get => lastCheckpoint; set => lastCheckpoint = value; }
    public GameObject DefaultCheckpoint { get => defaultCheckpoint; set => defaultCheckpoint = value; }

    private void Awake()
    {
        lastCheckpoint = defaultCheckpoint;
        SetHardMode();
        UpdateLives();
    }

    public void SetHardMode()
    {
        if(GameManager.instance.IsHardMode)
        {
            lives = 1;
        }
    }

    private void Update()
    {
        if(hasTakenDamage && health > 0)
        {
            TakeDamage();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.tag == "Heart" || collision.gameObject.tag == "Lava" || collision.gameObject.tag == "Rock" || collision.gameObject.tag == "Arrow" || (collision.gameObject.tag == "Spike" && Vector3.Dot((collision.gameObject.transform.position - transform.position).normalized, collision.gameObject.transform.up.normalized) < -0.01f)) && !hasTakenDamage)
        {
            hasTakenDamage = true;
            
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
        else if (collision.gameObject.tag == "DeathBox" )
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
        Time.timeScale = 0.5f;
        GetComponent<PlayerMovement>().EnableMovement(false);
        GetComponent<PlayerMovement>().IsGravityEnabled = false;
        Debug.Log("Player has died");
        AudioManager.instance.Play("playerdeath");
        GetComponent<Animator>().SetBool("IsDead", true);

        PlayerEvents.OnPlayerDeath(this, new PlayerDeathEventArgs(lastCheckpoint.GetComponent<Checkpoint>()));
        
        
    }

    public void OnDeathAnimationFinished()
    {
        Time.timeScale = 1f;
        if (canLoseLives) lives--;

        if (lives > 0)
        {

            UpdateLives();
            Respawn();
        }
        else
        {
            //Show Game Over Dialogue
            if (!GameObject.FindObjectOfType<Yarn.Unity.DialogueRunner>().IsDialogueRunning)
                GameObject.FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("playerdeath");
        }
    }

    public void AddLives(int lives)
    {
        this.lives += lives;
        UpdateLives();
    }

    private void UpdateLives()
    {
        liveText.text = lives.ToString();
    }

    private void Respawn()
    {
        
        if(lastCheckpoint == null)
        {
            lastCheckpoint = defaultCheckpoint;
        }
        transform.position = lastCheckpoint.transform.position;
        

        health = maxHealth;
        hasTakenDamage = false;
        GetComponent<Animator>().SetBool("IsDead", false);
        GetComponent<PlayerMovement>().EnableMovement(true);
        GetComponent<PlayerMovement>().IsGravityEnabled = true;

        if (GetComponent<PlayerMovement>().IsInverse)
        {
            GetComponent<PlayerMovement>().FlipGraivty();
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
