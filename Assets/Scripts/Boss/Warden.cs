using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warden : PatrolEnemy
{
    [Header("Health")]
    [SerializeField] private float currentHealth = 3f;
    [SerializeField] private float arrowCooldown = 1f;

    [SerializeField] private GameObject projectilePrefab;

    private float arrowTimer;

    private bool hasJumped = false;
    private bool isActive = false;

    public bool IsActive { get => isActive; set => isActive = value; }

    protected override void Update()
    {
        if (isActive)
        {
            base.Update();
            if (totalpatrolDistance < (patrolDistance / 2) + 0.01 && totalpatrolDistance >= (patrolDistance / 2) && !hasJumped)
            {
                isMoving = false;
                hasJumped = true;
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                arrowTimer = arrowCooldown;
            }

            if (IsGrounded())
            {
                isMoving = true;
                hasJumped = false;
            }
            else
            {
                arrowTimer -= Time.deltaTime;
                if (arrowTimer <= 0)
                {
                    ShootProjectile();
                    arrowTimer = arrowCooldown;
                }
            } 
        }
       

    }

    private void ShootProjectile()
    {
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        //Orient the Arrow
        Vector3 diff = player.transform.position - projectile.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 270);

        if (projectile.GetComponent<Rigidbody2D>() != null)
        {
            projectile.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * 10, ForceMode2D.Impulse);
        }
    }

    public bool IsGrounded()
    {
        float extraHeightTest = 0.05f;
        RaycastHit2D raycastHit;
        raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, extraHeightTest, mask);

        return raycastHit.collider != null && GetComponent<Rigidbody2D>().velocity.y < 0.01f;
    }

    public override IEnumerator Deactivate()
    {
        if(currentHealth - 1 <= 0)
        {
            Destroy(gameObject);
        }
        else
            currentHealth--;
        return base.Deactivate();
        
    }
}
