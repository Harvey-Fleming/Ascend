using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FlipLeverEvent : UnityEvent<GameObject, bool>
{

}


public class FinalBossLever : Lever
{

    public static FlipLeverEvent flipLever = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !HasUsed)
        {
            HasUsed = true;
            GetComponent<Animator>().SetTrigger("On");
            flipLever?.Invoke(gameObject, HasUsed);
        }
    }

    public override IEnumerator ResetLeverRoutine()
    {
        yield return new WaitForSeconds(DownTime);
        HasUsed = false;
        GetComponent<Animator>().SetTrigger("Off");
    }

    private void OnEnable()
    {
        FindObjectOfType<FinalBoss>().ResetLevers.AddListener(ResetLever);
    }
}
