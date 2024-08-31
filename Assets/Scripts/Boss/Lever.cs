using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    bool hasUsed = false;

    [SerializeField] float downTime = 1f;

    public bool HasUsed { get => hasUsed; set => hasUsed = value; }
    public float DownTime { get => downTime; set => downTime = value; }

    public virtual void ResetLever()
    {
        StartCoroutine(ResetLeverRoutine());
    }

    public virtual IEnumerator ResetLeverRoutine()
    {
        yield return new WaitForSeconds(downTime);
        hasUsed = false;
        GetComponent<Animator>().SetTrigger("Off");
    }
}
