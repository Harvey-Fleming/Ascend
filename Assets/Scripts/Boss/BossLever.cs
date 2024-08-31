using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLever : Lever
{
    [SerializeField] GameObject leverPlatform;

    public UnityEvent leverFlipped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !HasUsed)
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
}
