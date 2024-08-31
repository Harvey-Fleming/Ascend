using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float patrolDistance;
    [SerializeField] private float rayDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float deactivationTime = 1.5f;

    [SerializeField] LayerMask mask;

    Coroutine deactivationCoroutine;
    BoxCollider2D boxCollider;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (deactivationCoroutine == null)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.down, rayDistance, mask);
            Debug.DrawRay(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.down, Color.red, 0.1f);


            RaycastHit2D hitInfo2 = Physics2D.Raycast(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.right * transform.localScale.x, rayDistance, mask);
            Debug.DrawRay(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.right * transform.localScale.x, Color.blue, 0.1f);

            if (hitInfo.collider == null)
            {
                //Debug.Log("Did not Hit Down");
                moveSpeed *= -1;
                transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1);
            }

            if (hitInfo2.collider != null)
            {
                //Debug.Log("Hit " + hitInfo2.collider.name + " with tag " + hitInfo2.collider.tag + " Sideways");
                moveSpeed *= -1;
                transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1);
            }

            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if ((collision.gameObject.transform.position - boxCollider.bounds.max).y > 0)
            {
                if(deactivationCoroutine == null)
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

    IEnumerator Deactivate()
    {
        animator.SetBool("IsActivated", false);
        float elapsedTime = 0f;
        while(elapsedTime < deactivationTime)
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
