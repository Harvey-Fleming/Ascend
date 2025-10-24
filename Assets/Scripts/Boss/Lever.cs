using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    protected bool hasUsed = false;
    protected bool isEnabled = true;

    [SerializeField] float downTime = 1f;

    public bool HasUsed { get => hasUsed; set => hasUsed = value; }
    public float DownTime { get => downTime; set => downTime = value; }
    public bool IsEnabled { get => isEnabled; set => isEnabled = value; }

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
