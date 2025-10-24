using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLever : Lever
{
    [SerializeField] GameObject leverPlatform;

    public UnityEvent leverFlipped;

    public GameObject LeverPlatform { get => leverPlatform;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !HasUsed && IsEnabled)
        {
            HasUsed = true;
            GetComponent<Animator>().SetTrigger("On");
            if (leverPlatform != null)
            {
                leverPlatform.SetActive(true);
            }

            leverFlipped?.Invoke();
            StartCoroutine(ResetLeverRoutine());
        }
    }

    public override IEnumerator ResetLeverRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (leverPlatform != null)
        {
            leverPlatform.SetActive(false);
        }
        StartCoroutine(base.ResetLeverRoutine());
        yield return base.ResetLeverRoutine();


        yield return null;
    }

    public void SetLeverActive(bool isEnabled)
    {
        this.IsEnabled = isEnabled;

        if(!isEnabled)
            GetComponent<Animator>().SetTrigger("On");
        else
            GetComponent<Animator>().SetTrigger("Off");

    }
}
