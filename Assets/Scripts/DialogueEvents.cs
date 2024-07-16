using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueEvents : MonoBehaviour
{
    [SerializeField] List<GameObject> actors;

    List<GameObject> activeActors = new();

    #region - Move Commmands
    [YarnCommand("Move")]
    public static IEnumerator ForceMoveCoroutine(GameObject target, float newX, float duration, bool lookRight = true)
    {
        Vector3 startPos = target.transform.position;
        Animator animator = target.GetComponent<Animator>();
        float t = 0;
        while (t < duration)
        {
            target.transform.position = new Vector3(Mathf.Lerp(startPos.x, newX, t / duration), target.transform.position.y, target.transform.position.z);
            t += Time.deltaTime;

            if(animator != null)
            {
                animator.SetFloat("XSpeed", 1);
                animator.SetBool("IsGrounded", true);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if (Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }

            }

            yield return null;
        }
        target.transform.position = new Vector3(newX, target.transform.position.y, target.transform.position.z);
        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsGrounded", false);
        }

        yield return null;
    }

    [YarnCommand("MoveToActor")]
    public static IEnumerator ForceMoveToCoroutine(GameObject target, GameObject destination, float duration, bool lookRight = true)
    {
        Animator animator = target.GetComponent<Animator>();
        if(animator != null)
        {
            Debug.Log(target.name + " Has an animator");
        }
        Vector3 startPos = target.transform.position;
        Vector3 endPos = destination.transform.position;
        float t = 0;
        while (t < duration)
        {
            target.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, t / duration), target.transform.position.y, target.transform.position.z);
            t += Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("XSpeed", 1);
                animator.SetBool("IsGrounded", true);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if (Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }

            }

            yield return null;
        }
        target.transform.position = new Vector3(endPos.x, target.transform.position.y, target.transform.position.z);

        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsGrounded", false);
        }

        yield return null;
    }

    [YarnCommand("MoveInDirection")]
    public static IEnumerator ForceMoveInDirCoroutine(GameObject target, float xDir, float duration, bool lookRight = true)
    {
        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log(target.name + " Has an animator");
        }
        Vector3 startPos = target.transform.position;
        Vector3 endPos = target.transform.position + new Vector3(xDir, 0.0f, 0.0f);
        float t = 0;
        while (t < duration)
        {
            target.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, t / duration), target.transform.position.y, target.transform.position.z);
            t += Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("XSpeed", 1);
                animator.SetBool("IsGrounded", true);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if (Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }

            }

            yield return null;
        }
        target.transform.position = new Vector3(endPos.x, target.transform.position.y, target.transform.position.z);

        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsGrounded", false);
        }

        yield return null;
    }

    [YarnCommand("MoveInDirectionMethod")]
    public void MoveInDirMethod(GameObject target, float xDir, float duration, bool lookRight = true)
    {
        StartCoroutine(ForceMoveInDirCoroutine(target, xDir, duration, lookRight));
    }
    #endregion

    #region - Fly Commmands
    [YarnCommand("Fly")]
    public static IEnumerator ForceFlyCoroutine(GameObject target, float x, float y, float z, float duration, bool lookRight = true)
    {
        Vector3 startPos = target.transform.position;
        Vector3 endPos = new Vector3(x, y, z);
        Animator animator = target.GetComponent<Animator>();
        float t = 0;
        while (t < duration)
        {
            target.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("YSpeed", 1);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if(Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }
                
            }

            yield return null;
        }
        target.transform.position = endPos;

        if (animator != null)
        {
            animator.SetFloat("YSpeed", 0);
        }

        yield return null;
    }

    [YarnCommand("FlyToActor")]
    public static IEnumerator ForceFlyToCoroutine(GameObject target, GameObject destination, float duration, bool lookRight = true)
    {
        Animator animator = target.GetComponent<Animator>();
        Vector3 startPos = target.transform.position;
        Vector3 endPos = destination.transform.position;
        float t = 0;
        while (t < duration)
        {
            target.transform.position = Vector3.Lerp(startPos, endPos, t/duration);
            t += Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("YSpeed", 1);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if (Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }

            }

            yield return null;
        }
        target.transform.position = endPos;

        if (animator != null)
        {
            animator.SetFloat("YSpeed", 0);
        }

        yield return null;
    }

    [YarnCommand("FlyInDirection")]
    public static IEnumerator ForceFlyInDirCoroutine(GameObject target, float xDir, float yDir, float duration, bool lookRight = true)
    {
        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log(target.name + " Has an animator");
        }
        Vector3 startPos = target.transform.position;
        Vector3 endPos = target.transform.position + new Vector3(xDir, yDir, 0.0f);
        float t = 0;
        while (t < duration)
        {
            target.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("XSpeed", 1);
                animator.SetBool("IsGrounded", true);
            }

            if (lookRight)
            {
                Vector3 scale = new Vector3(Mathf.Abs(target.transform.localScale.x), target.transform.localScale.y, target.transform.localScale.z);
                target.transform.localScale = scale;
            }
            else
            {
                if (Mathf.Sign(target.transform.localScale.x) > 0)
                {
                    Vector3 scale = new Vector3(-target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
                    target.transform.localScale = scale;
                }

            }

            yield return null;
        }
        target.transform.position = new Vector3(endPos.x, target.transform.position.y, target.transform.position.z);

        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsGrounded", false);
        }

        yield return null;
    }

    [YarnCommand("Fly2InDirection")]
    public void StartFly2InDirection(GameObject target1, GameObject target2, float x, float y, float duration, bool lookright = true)
    {
        StartCoroutine(ForceFlyInDirCoroutine(target1, x, y, duration, lookright));
        StartCoroutine(ForceFlyInDirCoroutine(target2, x, y, duration, lookright));
    }
    #endregion

    #region - Spawn Actors
    [YarnCommand("SpawnActor")]
    public void SpawnActor(string actorName, float x, float y, float z)
    {
        foreach(GameObject actor in actors)
        {
            if(actorName.ToString() == actor.name)
            {
                GameObject newActor = Instantiate(actor, new Vector3(x, y, z), Quaternion.identity);
                newActor.name = actorName;
                activeActors.Add(newActor);
            }
        }
    }

    [YarnCommand("SpawnActorAt")]
    public void SpawnActor(GameObject targetObject, string actorName)
    {
        foreach (GameObject actor in actors)
        {
            if (actorName.ToString() == actor.name)
            {
                GameObject newActor = Instantiate(actor, new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z), Quaternion.identity);
                newActor.name = actorName;
                activeActors.Add(newActor);
            }
        }
    }

    [YarnCommand("DestroyActor")]
    public void DestroyActor(string actorName)
    {
        GameObject actor = GameObject.Find(actorName);
        if (activeActors.Contains(actor))
        {
            activeActors.Remove(actor);
            Destroy(actor);
        }
    }
    #endregion

    [YarnCommand("StartFire")]
    public static void StartFires()
    {
        GameObject fireParent = GameObject.Find("Fires");

        foreach(Transform child in fireParent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    [YarnCommand("CompleteLevel")]
    public static void LoadNextLevel()
    {
        GameManager.instance.LoadNextLevel();
    }




}

