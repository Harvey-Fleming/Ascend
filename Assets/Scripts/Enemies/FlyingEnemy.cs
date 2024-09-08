using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    Transform gun;
    GameObject player;

    [SerializeField] private float projectileCooldown = 1f;
    [SerializeField] private GameObject projectilePrefab;
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
            GameObject projectile = Instantiate(projectilePrefab, gun.transform.position, gun.transform.rotation);
            projectile.transform.localScale = Vector3.one;
            yield return null;
        }

    }
}
