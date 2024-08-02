using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseBehaviour : MonoBehaviour
{
    bool ischasing = false;

    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseOffset = 5f;

    List<BezierKnot> waypoints = new();

    float chaseCounter = 0f;
    int currentKnot = 0;

    public void OnBeginChase(ChaseEventArgs args)
    {
        Debug.Log("Chase Start Event Triggered");
        ischasing = true;
        StartCoroutine(ChaseCoroutine(args));
    }

    public IEnumerator ChaseCoroutine(ChaseEventArgs args)
    {
        Vector3 NefariumBasePos = args.Nefarium.transform.position;
        Vector3 FriendBasePos = args.Friend.transform.position;

        foreach (BezierKnot knot in args.Nefarium.GetComponent<SplineAnimate>().Container.Spline.Knots.ToList())
        {
            waypoints.Add(knot);
        }

        while (ischasing)
        {
            Vector3 knotPos = new Vector3(waypoints[currentKnot].Position.x, waypoints[currentKnot].Position.y, waypoints[currentKnot].Position.z) + args.Nefarium.GetComponent<SplineAnimate>().Container.gameObject.transform.position;

            if (Vector3.Distance(args.Nefarium.transform.position, knotPos) > 0.01f)
            {
                //Move forward Along the Spline
                chaseCounter += Time.deltaTime * chaseSpeed;
                args.Nefarium.transform.position = Vector3.Lerp(NefariumBasePos, knotPos, chaseCounter); 
            }
            else 
            {
                if (Vector3.Distance(GameObject.Find("Player").transform.position, args.Nefarium.transform.position) <= 3)
                {
                    chaseCounter = 0f;
                    currentKnot++;
                    NefariumBasePos = args.Nefarium.transform.position;

                    if(currentKnot >= waypoints.Count)
                    {
                        ischasing = false;
                        Debug.Log("Chase finished");
                    }
                }
            }



            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void ResetChase(object sender, PlayerDeathEventArgs args)
    {
        if(ischasing)
        {
            GameObject Nefarium = GameObject.Find("Nefarium");
            Debug.Log("Reset Chase");
            Nefarium.GetComponent<SplineAnimate>().Restart(false);
            chaseCounter = 0;
            currentKnot = 0;
        }
    }

    private void OnEnable()
    {
        PlayerEvents.PlayerDeath += ResetChase;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayerDeath -= ResetChase;
    }
}

public class ChaseEventArgs
{
    public GameObject Nefarium;
    public GameObject Friend;

    public ChaseEventArgs(GameObject Nefarium, GameObject Friend)
    {
        this.Nefarium = Nefarium;
        this.Friend = Friend;
    }

}
