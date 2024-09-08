using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float patrolDistance;
    private float totalpatrolDistance;
    [SerializeField] private float rayDistance;
    [SerializeField] private float moveSpeed;

    [SerializeField] LayerMask mask;

    public override void Start()
    {
        base.Start();
        totalpatrolDistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (deactivationCoroutine == null)
        {
            //Raycast diagnonally down to detect an edge
            RaycastHit2D hitInfo = Physics2D.Raycast(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.down, rayDistance, mask);
            Debug.DrawRay(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.down, Color.red, 0.1f);

            //Raycast right to detect a collision
            RaycastHit2D hitInfo2 = Physics2D.Raycast(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.right * transform.localScale.x, rayDistance, mask);
            Debug.DrawRay(Mathf.Sign(transform.localScale.x) > 0 ? new Vector2(boxCollider.bounds.max.x + 0.2f, boxCollider.bounds.min.y + 0.1f) : new Vector2(boxCollider.bounds.min.x - 0.2f, boxCollider.bounds.min.y + 0.1f), Vector2.right * transform.localScale.x, Color.blue, 0.1f);

            if (hitInfo.collider == null || totalpatrolDistance > patrolDistance)
            {
                //Debug.Log("Did not Hit Down");
                moveSpeed *= -1;
                transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1);
                totalpatrolDistance = 0;
            }

            if (hitInfo2.collider != null)
            {
                //Debug.Log("Hit " + hitInfo2.collider.name + " with tag " + hitInfo2.collider.tag + " Sideways");
                moveSpeed *= -1;
                transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1);
                totalpatrolDistance = 0;
            }

            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f);
            totalpatrolDistance += Mathf.Abs(moveSpeed * Time.deltaTime);
        }
    }


}
