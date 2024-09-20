using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    Transform gun;
    GameObject player;

    [SerializeField] private float projectileCooldown = 1f;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float turretRange = 5f;
    // Start is called before the first frame update
    void Start()
    {
        gun = transform.GetChild(0);
        player = GameObject.Find("Player");
        StartCoroutine(ShootProjectile());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = player.transform.position - gun.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
    }

    private IEnumerator ShootProjectile()
    {
        while(gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(projectileCooldown);
            if(Vector3.Distance(transform.position, player.transform.position) <= turretRange)
            {
                GameObject projectile = Instantiate(projectilePrefab, gun.transform.position, gun.transform.rotation);
                projectile.GetComponent<ArrowCollision>().CanHitGround = false;
                projectile.GetComponent<Collider2D>().enabled = false;
                projectile.transform.rotation = Quaternion.Euler(gun.transform.rotation.eulerAngles + new Vector3(0, 0, 180));

                if(projectile.GetComponent<Rigidbody2D>() != null)
                {
                    projectile.GetComponent<Rigidbody2D>().AddForce((player.transform.position - gun.transform.position).normalized * 10, ForceMode2D.Impulse);
                }
                


                
                yield return new WaitForSeconds(0.1f);
                projectile.GetComponent<Collider2D>().enabled = true;
                projectile.GetComponent<ArrowCollision>().CanHitGround = true;
            }
            yield return null;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, turretRange);
    }
}
