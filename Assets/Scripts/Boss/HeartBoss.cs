using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBoss : MonoBehaviour
{
    public bool hasStarted;

    int health = 3;
    [SerializeField] int maxhealth = 3;

    [SerializeField] ParticleSystem bloodpSys1;
    [SerializeField] ParticleSystem bloodpSys2;


    [SerializeField] ParticleSystem BeamPSys;
    [SerializeField] private float beamSpeed;

    [Space]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 1f;
    GameObject[] arrows = new GameObject[5];

    [Space]
    [SerializeField] Image blackImage;

    HeartState currentAttack = HeartState.idle;

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

    private void Start()
    {
        FindObjectOfType<PlayerHealth>().playerDeathEvent.AddListener(ResetBoss);
    }

    private void Update()
    {
        if(currentAttack == HeartState.idle && hasStarted)
        {
            NextAttack();
        }

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    StopCoroutine(beamAttack);
        //    NextAttack();
        //}
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
        BeamPSys.Play();
        while(BeamPSys.shape.rotation.x < 360)
        {
            ParticleSystem.ShapeModule beamShape = BeamPSys.shape;
            Vector3 beamRot = BeamPSys.shape.rotation;
            beamRot.x += 1 * Time.deltaTime * beamSpeed;
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

            arrows[i] = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
            arrows[i].tag = "Heart";

            while(Vector3.Distance(pPos, arrows[i].transform.position) > 0.5f)
            {                  
                arrows[i].transform.position += dir.normalized * arrowSpeed * Time.deltaTime;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrows[i].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                yield return new WaitForSeconds(0.01f);
            }
        }
        arrowAttack = null;
        yield return null;
    }

    public void StartFight()
    {
        hasStarted = true;
    }

    public void TakeDamage()
    {
        if(hasStarted)
        Debug.Log("Boss has Taken Damage");
        health--;

        if(health <= 0)
        {
            Death();
        }

    }

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

        bloodpSys1.Play();
        bloodpSys2.Play();

        FindObjectOfType<TimeManager>().StopTimer();

        StartCoroutine("FadeToBlack");
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        while (blackImage.color.a <= 0.9)
        {
            t += 1 * Time.deltaTime;
            blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.Lerp(blackImage.color.a, 1, t));
            Debug.Log(blackImage.color.a);
            yield return new WaitForSeconds(0.1f);
        }
        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, 1);
        FindObjectOfType<FinalScreen>().OnShowScreen(FindObjectOfType<TimeManager>().GetTimer(), FindObjectOfType<CollectCoin>().CoinsGathered);
        yield return null;
    }

    public void ResetBoss()
    {
        if(hasStarted)
        {
            Debug.Log("Boss Reset");
            health = maxhealth;
            if(arrowAttack != null)
                StopCoroutine(arrowAttack);

            foreach (GameObject arrow in arrows)
            {
                Destroy(arrow);
            }
            BossLever[] levers = FindObjectsOfType<BossLever>();

            foreach (BossLever lever in levers)
            {
                lever.Reset();
            }

            hasStarted = false;
            FindObjectOfType<HeartBossTrigger>().Reset();
            currentAttack = HeartState.idle;
        }

    }
}