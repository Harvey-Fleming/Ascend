using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NefariumBoss : MonoBehaviour
{
    bool isActive = false;

    [SerializeField] State currentState;

    [SerializeField] LayerMask beamlayerMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] private float beamSpeed = 10f;

    public LayerMask BeamlayerMask { get => beamlayerMask; set => beamlayerMask = value; }
    public LayerMask PlayerMask { get => playerMask; set => playerMask = value; }
    public State CurrentState { get => currentState; set => currentState = value; }
    public float BeamSpeed { get => beamSpeed; set => beamSpeed = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            currentState.Execute();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isActive)
        {
            isActive = true;
            currentState = new NefariumIdleState(this);
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

    protected NefariumBoss controller;

    protected State(NefariumBoss controller)
    {
        this.controller = controller;
    }
}

public class NefariumIdleState : State
{
    public NefariumIdleState(NefariumBoss controller) : base(controller)
    {
        Debug.Log("Idle State");
    }

    public override void Execute()
    {
        controller.CurrentState = new NefariumBeamState(controller);
    }
}

public class NefariumRockDropState : State
{
    public NefariumRockDropState(NefariumBoss controller) : base(controller)
    {
        Debug.Log("RockDrop State");
    }

    public override void Execute()
    {
        
    }
}

public class NefariumBeamState : State
{
    Coroutine beamAttack = null;
    
    private float beamCounter = 0f;
    public NefariumBeamState(NefariumBoss controller) : base(controller)
    {
        Debug.Log("Beam State");
    }

    public override void Execute()
    {
        if(beamAttack == null)
        {
            beamAttack = controller.StartCoroutine(BeamAttack());
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

            if (hit = Physics2D.Raycast(controller.transform.position, v.normalized, Mathf.Infinity, controller.BeamlayerMask))
            {
                renderer.SetPosition(1, hit.point);
            }

            RaycastHit2D[] hitAll;

            if ((hitAll = Physics2D.RaycastAll(controller.transform.position, v.normalized, 25, controller.PlayerMask)).Length != 0)
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

            beamCounter += controller.BeamSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.15f);
        }

        controller.CurrentState = new NefariumRockDropState(controller);
        beamAttack = null;
        yield return null;
    }
}
