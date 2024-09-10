using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
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

    [YarnCommand("ChangeSpline")]
    public static void ChangeSpline(GameObject target1, GameObject splineObject)
    {
        target1.GetComponent<SplineAnimate>().Container = splineObject.GetComponent<SplineContainer>();
 
    }

    [YarnCommand("ResetSpline")]
    public static void ResetSpline(GameObject target1)
    {
        target1.GetComponent<SplineAnimate>().Restart(false);
    }

    [YarnCommand("FollowSpline")]
    public static void FollowSpline(GameObject target1)
    {
        target1.GetComponent<SplineAnimate>().Play();
    }

    [YarnCommand("TeleportActor")]
    public static void TeleportActor(GameObject target1, float x, float y, float z)
    {
        target1.transform.position = new Vector3(x, y, z);
        
    }
    #endregion

    #region - Jump Command
    
    [YarnCommand("JumpInDirection")]
    public static void JumpInDirection(GameObject actor, float xDir, float yDir, float jumpPower)
    {
        if(actor.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            actor.GetComponent<Animator>().SetTrigger("Final Boss Jump");
        }
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
        else
        {
            Destroy(actor);
        }
    }
    #endregion

    #region - Village Specific Dialogue Events
    [YarnCommand("StartFire")]
    public static void StartFires()
    {
        GameObject fireParent = GameObject.Find("Fires");

        foreach(Transform child in fireParent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    [YarnCommand("SetNightTime")]
    public static void SetNightTime()
    {
        GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = 0.45f;

        Color nightBackground = new Color(24, 45, 63);
        Camera.main.backgroundColor = nightBackground;
    }
    #endregion

    #region - MaintainPass Specific Dialogue Events

    [YarnCommand("ShootProjectile")]
    public static IEnumerator MoveProjectile(float duration)
    {
        GameObject projectile = GameObject.Find("Projectile");
        Vector3 startPos = projectile.transform.position;
        float t = 0;
        while (t < duration)
        {
            projectile.transform.position = new Vector3(projectile.transform.position.x, Mathf.Lerp(startPos.y, 5, t / duration), projectile.transform.position.z);
            t += Time.deltaTime;
            yield return null;
        }
        projectile.transform.position = new Vector3(projectile.transform.position.x, 5, projectile.transform.position.z);
        GameObject.FindObjectOfType<OrbAnimation>().OnPedestalBreak();

        Destroy(projectile);

        GameObject Friend = GameObject.Find("Friend");
        GameObject Nefarium = GameObject.Find("Nefarium");

        Friend.transform.SetParent(null, true);

        Nefarium.transform.position = new Vector3(45.66f, -11.59f, 0f);
        Friend.transform.position = new Vector3(46.5f, -11.59f, 0f);
    }

    [YarnCommand("StartNefariumBoss")]
    public static void StartFight()
    {
        GameObject.FindObjectOfType<NefariumBoss>().StartFight();

    }


    [YarnCommand("StartChase")]
    public static void StartChase()
    {
        GameObject Friend = GameObject.Find("Friend");
        GameObject Nefarium = GameObject.Find("Nefarium");

        Friend.transform.SetParent(Nefarium.transform, true);
        Friend.transform.localPosition = new Vector3(0f, -0.444f, 0f);


        Nefarium.GetComponent<SplineAnimate>().Container = GameObject.Find("NefariumChaseSpline").GetComponent<SplineContainer>();

        GameObject.Find("ChaseManager").GetComponent<ChaseBehaviour>().OnBeginChase(new ChaseEventArgs(GameObject.Find("Nefarium"), GameObject.Find("Friend")));
    }

    #endregion

    [YarnCommand("CompleteLevel")]
    public static void LoadNextLevel()
    {
        AudioManager.instance.StopAll();
        GameManager.instance.LoadNextLevel();
        
    }
    [YarnCommand("ReloadLevel")]
    public static void ReloadLevel()
    {
        AudioManager.instance.StopAll();
        GameManager.instance.ReloadLevel();
    }

    [YarnCommand("LoadLevel")]
    public static void LoadLevel(int index)
    {
        AudioManager.instance.StopAll();
        GameManager.instance.LoadLevel(index);
    }

    [YarnCommand("PlayMusic")]
    public static void PlayMusic(string name)
    {
        AudioManager.instance.Play(name);
    }

    [YarnCommand("FadeOutMusic")]
    public static void FadeOutMusic(string fadeOutName, int fadeDuration)
    {
        AudioManager.instance.FadeOutSound(fadeOutName, fadeDuration);
    }

    [YarnCommand("FadeInMusic")]
    public static void FadeInMusic(string fadeInname, int fadeDuration)
    {
        AudioManager.instance.FadeInSound(fadeInname, fadeDuration);
    }




}

