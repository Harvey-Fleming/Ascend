using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBoss : MonoBehaviour
{
    [Header("State")]
    public bool hasStarted;
    [SerializeField] HeartState currentAttack = HeartState.idle;

    [Header("Health")]
    [SerializeField] int currentHealth = 3;
    [SerializeField] int maxHealth = 3;

    [Space]
    [Header("Particle Systems")]
    [SerializeField] ParticleSystem leftBloodPSys;
    [SerializeField] ParticleSystem rightBloodPSys;
    [SerializeField] ParticleSystem BeamPSys;

    [Space]
    [Header("Beam Variables")]
    [SerializeField] private float beamSpeed;

    [Space]
    [Header("Arrow Variables")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 1f;
    GameObject[] arrows = new GameObject[5];

    [Space]
    [SerializeField] Image blackImage;

    private float maxIdleSwitchTimer = 1f;
    private float idleSwitchTimer = 0f;

    [SerializeField] Sprite deathSprite;

    Coroutine beamAttack;
    Coroutine arrowAttack;

    public enum HeartState
    { 
        idle,
        Beam,
        Projectiles,
        Death,
    }

    private void Update()
    {
        if(currentAttack == HeartState.idle && hasStarted && idleSwitchTimer >= maxIdleSwitchTimer)
        {
            NextAttack();
            idleSwitchTimer = 0;
        }
        else if(currentAttack == HeartState.idle && hasStarted)
        {
            idleSwitchTimer += Time.deltaTime;
        }
    }

    private void NextAttack()
    {
        int currentAttackint = (int)currentAttack;
        currentAttackint++;
        currentAttack = (HeartState)currentAttackint;

        switch (currentAttack)
        {
            case HeartState.idle:
                break;
            case HeartState.Beam:
                beamAttack = StartCoroutine("BeamAttack");
                break;
            case HeartState.Projectiles:
                arrowAttack = StartCoroutine("ArrowAttack");
                break;
        }
    }

    IEnumerator BeamAttack()
    {
        ParticleSystem.ShapeModule beamShape = BeamPSys.shape;
        Vector3 beamRot = BeamPSys.shape.rotation;
        beamRot.x = 0;
        beamShape.rotation = beamRot;

        BeamPSys.Play();
        while(BeamPSys.shape.rotation.x < 360)
        {
            beamShape = BeamPSys.shape;
            beamRot = BeamPSys.shape.rotation;
            beamRot.x += beamSpeed * 0.01f;
            beamShape.rotation = beamRot;
            yield return new WaitForSeconds(0.01f);
        }
        BeamPSys.Stop();
        NextAttack();
        beamAttack = null;
        yield return null;
    }

    IEnumerator ArrowAttack()
    {
        
        for(int i = 0; i < 5; i++)
        {
            Vector3 pPos = GameObject.FindWithTag("Player").transform.position;
            Vector3 dir = pPos - this.transform.position;

            if(arrows[i] == null)
            {
                arrows[i] = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
                arrows[i].tag = "Heart";
                arrows[i].layer = 8;
            }
            else
            {
                arrows[i].layer = 8;
                arrows[i].transform.position = this.transform.position;
            }

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrows[i].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            if (arrows[i].GetComponent<Rigidbody2D>() != null)
            {
                arrows[i].GetComponent<Rigidbody2D>().AddForce((dir).normalized * 10, ForceMode2D.Impulse);
                arrows[i].GetComponent<ArrowCollision>().Direction = dir;
            }
            yield return new WaitForSeconds(0.5f);
        }
        arrowAttack = null;
        currentAttack = HeartState.idle;
        yield return null;
    }

    [ContextMenu("Start Boss")]
    public void StartFight()
    {
        hasStarted = true;
    }

    public void TakeDamage()
    {
        if(hasStarted)
        {
            Debug.Log("Boss has Taken Damage");
            currentHealth--;

            foreach(GameObject arrow in arrows)
            {
                arrow.SetActive(false);
            }

            if (arrowAttack != null)
            {
                StopCoroutine(arrowAttack);
            }

            StopBeam();

            currentAttack = HeartState.idle;


            if (currentHealth <= 0)
            {
                Death();
            }
        }
    }

    [ContextMenu("Kill Boss")]
    private void Death()
    {
        currentAttack = HeartState.Death;
        GetComponent<SpriteRenderer>().sprite = deathSprite;

        if(beamAttack != null)
        {
            StopCoroutine(beamAttack);
            BeamPSys.Stop();

            ParticleSystem.ShapeModule beamShape = BeamPSys.shape;
            Vector3 beamRot = BeamPSys.shape.rotation;
            beamRot.x = 0;
            beamShape.rotation = beamRot;
        }

        GetComponent<CircleCollider2D>().enabled = false;

        leftBloodPSys.Play();
        rightBloodPSys.Play();

        FindObjectOfType<TimeManager>().StopTimer();

        GameManager.instance.StartFadeToBlack();
        FindObjectOfType<FinalScreen>().OnShowScreen(FindObjectOfType<TimeManager>().GetTimer(), FindObjectOfType<CollectCoin>().CoinsGathered);
    }

    public void ResetBoss(object sender, PlayerDeathEventArgs args)
    {
        if(hasStarted)
        {
            Debug.Log("Boss Reset");
            currentHealth = maxHealth;
            if (arrowAttack != null)
                StopCoroutine(arrowAttack);
            StopBeam();

            foreach (GameObject arrow in arrows)
            {
                Destroy(arrow);
            }
            BossLever[] levers = FindObjectsOfType<BossLever>();

            foreach (BossLever lever in levers)
            {
                lever.ResetLever();
            }

            hasStarted = false;
            FindObjectOfType<HeartBossTrigger>().Reset();
            currentAttack = HeartState.idle;
        }

    }

    private void StopBeam()
    {
        if (beamAttack != null)
            StopCoroutine(beamAttack);

        BeamPSys.Stop();
        ParticleSystem.ShapeModule beamShape = BeamPSys.shape;
        Vector3 beamRot = BeamPSys.shape.rotation;
        beamRot.x = 0;
        beamShape.rotation = beamRot;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Heart Hit: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Heart") && Vector3.Dot(collision.gameObject.GetComponent<Rigidbody2D>().velocity.normalized, (collision.gameObject.transform.position - transform.position).normalized) == 0)
        {
            Debug.Log("Hit by arrow");
            TakeDamage();
        }
    }
    private void OnEnable()
    {
        PlayerEvents.PlayerDeath += ResetBoss;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayerDeath -= ResetBoss;
    }
}