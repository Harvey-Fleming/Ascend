using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float deactivationTime = 1.5f;
    protected Coroutine deactivationCoroutine;

    protected BoxCollider2D boxCollider;
    protected Animator animator;

    // Start is called before the first frame update
    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((collision.gameObject.transform.position - boxCollider.bounds.max).y > 0)
            {
                Rigidbody2D playerrb = collision.gameObject.GetComponent<Rigidbody2D>();
                playerrb.velocity = new Vector2(playerrb.velocity.x, 0);
                playerrb.AddForce(Vector2.up * 13, ForceMode2D.Impulse);
                if (deactivationCoroutine == null)
                {
                    deactivationCoroutine = StartCoroutine(Deactivate());
                }
            }
            else
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            }
        }
    }

    public virtual IEnumerator Deactivate()
    {
        animator.SetBool("IsActivated", false);
        float elapsedTime = 0f;
        while (elapsedTime < deactivationTime)
        {
            //Robot is deactivated
            boxCollider.enabled = false;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("IsActivated", true);
        boxCollider.enabled = true;
        deactivationCoroutine = null;
        yield return null;
    }

}
