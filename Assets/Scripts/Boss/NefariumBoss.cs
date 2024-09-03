using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NefariumBoss : Boss
{
    bool isActive = false;

    [SerializeField] State currentState;

    [SerializeField] LayerMask beamlayerMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] private float beamSpeed = 10f;

    [SerializeField] private GameObject rubbleParticles;

    public LayerMask BeamlayerMask { get => beamlayerMask; set => beamlayerMask = value; }
    public LayerMask PlayerMask { get => playerMask; set => playerMask = value; }
    public State CurrentState { get => currentState; set => currentState = value; }
    public float BeamSpeed { get => beamSpeed; set => beamSpeed = value; }
    public GameObject RubbleParticles { get => rubbleParticles; set => rubbleParticles = value; }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            currentState.Execute();
        }
    }

    public void StartFight()
    {
        if(!isActive)
        {
            isActive = true;
            currentState = new NefariumIdleState(this);
            GameObject.Find("Boss Door").GetComponent<Animator>().SetTrigger("CloseDoor");
        }

    }

    public override void TakeDamage()
    {
        base.TakeDamage();

        if (currentHealth <= 0)
        {
            currentState = new NefariumDeathState(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if ((collision.gameObject.transform.position - GetComponent<Collider2D>().bounds.max).y > 0)
            {
                //Take away health
                Debug.Log("Nefarium Damage");
                TakeDamage();
            }
            else
            {
                Debug.Log("Nefarium Player Damage");
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            }
        }
    }
}

public interface IState
{
    public abstract void Execute();
}

//Defines abstract class for monster State Machine
[System.Serializable]
public abstract class State : IState
{
    public abstract void Execute();

    public abstract void EndState();

    protected Boss controller;

    protected State(Boss controller)
    {
        this.controller = controller;
    }


}

public class NefariumIdleState : State
{
    public NefariumIdleState(Boss controller) : base(controller)
    {
        Debug.Log("Idle State");
    }

    public override void EndState()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        //controller.CurrentState = new NefariumBeamState(controller);
        ((NefariumBoss)controller).CurrentState = new NefariumBeamState(controller);
    }
}

public class NefariumDeathState : State
{
    public NefariumDeathState(Boss controller) : base(controller)
    {
        Debug.Log("Death State");
        controller.GetComponent<SpriteRenderer>().enabled = false;
        controller.GetComponent<Collider2D>().enabled = false;
        GameObject.FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("bossend");
    }

    public override void EndState()
    {
    }

    public override void Execute()
    {
    }
}

public class NefariumRockDropState : State
{
    GameObject rockLeft;
    GameObject rockRight;

    Vector3 leftRockBasePos;
    Vector3 rightRockBasePos;

    float t = 0f;
    float particleT = 0f;
    float exitT = 0f;


    float particleDuration = 2f;
    float phaseDuration = 4f;

    bool hasChangedBase = false;

    List<GameObject> particles = new();

    public NefariumRockDropState(Boss controller) : base(controller)
    {
        Debug.Log("RockDrop State");
        rockLeft = GameObject.Find("Boss Rubble Left");
        leftRockBasePos = rockLeft.transform.position;

        rockRight = GameObject.Find("Boss Rubble Right");
        rightRockBasePos = rockRight.transform.position;

        particles.Add(GameObject.Instantiate(((NefariumBoss)controller).RubbleParticles, rockLeft.transform.position, Quaternion.identity));
        particles.Add(GameObject.Instantiate(((NefariumBoss)controller).RubbleParticles, rockRight.transform.position, Quaternion.identity));
    }

    public override void EndState()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {    
        particleT += Time.deltaTime;

        if(particleT > particleDuration)
        {
            t += Time.deltaTime;
            if (t >= phaseDuration)
            {
                if(!hasChangedBase)
                {
                    hasChangedBase = true;
                    leftRockBasePos = rockLeft.transform.position;
                    rightRockBasePos = rockRight.transform.position;
                }
                exitT += Time.deltaTime;

                rockLeft.transform.position = Vector3.Lerp(leftRockBasePos, leftRockBasePos - (Vector3.up * 14.5f), exitT / 2);
                rockRight.transform.position = Vector3.Lerp(rightRockBasePos, rightRockBasePos - (Vector3.up * 14.5f), exitT / 2);

                if(exitT > 2)
                {
                    rockLeft.transform.position = new Vector3(rockLeft.transform.position.x, 39, rockLeft.transform.position.z);
                    rockRight.transform.position = new Vector3(rockRight.transform.position.x, 38, rockRight.transform.position.z);
                    ((NefariumBoss)controller).CurrentState = new NefariumIdleState(controller);
                }
            }
            else
            {
                
                foreach (GameObject particle in particles)
                {
                    GameObject.Destroy(particle);
                }
                rockLeft.transform.position = Vector3.Lerp(leftRockBasePos, leftRockBasePos - (Vector3.up * 14.5f), t);
                rockRight.transform.position = Vector3.Lerp(rightRockBasePos, rightRockBasePos - (Vector3.up * 14.5f), t);


            }
        }


    }
}

public class NefariumBeamState : State
{
    Coroutine beamAttack = null;
    
    private float beamCounter = 0f;
    public NefariumBeamState(Boss controller) : base(controller)
    {
        Debug.Log("Beam State");
        FlipColliderState();


    }

    public override void Execute()
    {
        if(beamAttack == null)
        {
            beamAttack = controller.StartCoroutine(BeamAttack());
        }

    }

    private void FlipColliderState()
    {
        GameObject leverChild = controller.transform.GetChild(0).gameObject;
        foreach (Transform child in leverChild.transform)
        {
            if(!child.GetComponent<Collider2D>().enabled == false)
            {
                
                child.GetComponent<Animator>().SetBool("Power", false);
            }
            else if(!child.GetComponent<Collider2D>().enabled == true)
            {
                child.GetComponent<Animator>().SetBool("Power", true);
            }
            child.GetComponent<Collider2D>().enabled = !child.GetComponent<Collider2D>().enabled;
        }
    }

    IEnumerator BeamAttack()
    {
        LineRenderer renderer = controller.GetComponent<LineRenderer>();
        renderer.enabled = true;
        while (beamCounter <= 6.283)
        {

            Vector3 v = Quaternion.AngleAxis(beamCounter * 180/Mathf.PI, Vector3.forward) * Vector3.up;

            RaycastHit2D hit; 
            Debug.DrawLine(controller.transform.position, controller.transform.position + v.normalized, Color.red, 0.5f);

            if (hit = Physics2D.Raycast(controller.transform.position, v.normalized, Mathf.Infinity, ((NefariumBoss)controller).BeamlayerMask))
            {
                renderer.SetPosition(1, hit.point);
            }

            RaycastHit2D[] hitAll;

            if ((hitAll = Physics2D.RaycastAll(controller.transform.position, v.normalized, 25, ((NefariumBoss)controller).PlayerMask)).Length != 0)
            {
                GameObject closestObject = null;
                float closestDistance = float.MaxValue;
         
                foreach(RaycastHit2D hits in hitAll)
                {
                    //Debug.Log("Hit collider is " + hits.collider.name);
                    //Debug.Log("controller collider is " + controller.gameObject.name);

                    if(hits.collider.isTrigger || hits.collider.name == controller.gameObject.name || (!hits.collider.CompareTag("Ground") && !hits.collider.CompareTag("Player")))
                    {
                        continue;
                    }
                    Debug.Log(hit.collider.name);
                    if((controller.transform.position - hits.collider.transform.position).sqrMagnitude < closestDistance)
                    {
                        closestObject = hits.collider.gameObject;
                        closestDistance = (controller.transform.position - hits.collider.transform.position).sqrMagnitude;
                    }

                }

                //Debug.Log(closestObject.name);
                if (closestObject != null && closestObject.CompareTag("Player"))
                {
                    
                    closestObject.GetComponent<PlayerHealth>().TakeDamage();
                }
            }

            beamCounter += ((NefariumBoss)controller).BeamSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.15f);
        }

        ((NefariumBoss)controller).CurrentState = new NefariumRockDropState(controller);
        FlipColliderState();
        beamAttack = null;
        yield return null;
    }

    public override void EndState()
    {
        throw new System.NotImplementedException();
    }
}
