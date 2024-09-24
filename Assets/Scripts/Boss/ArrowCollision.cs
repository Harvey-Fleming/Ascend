using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    [SerializeField] private bool canHitGround = true;
    [SerializeField] private bool destroyOnCollision = false;

    private Vector3 direction;

    [SerializeField] private LayerMask groundLayers;

    public bool CanHitGround { get => canHitGround; set => canHitGround = value; }
    public Vector3 Direction { get => direction; set => direction = value; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Arrow Hit: " + collision.gameObject.name);
        if(collision.gameObject.CompareTag("Ground") && canHitGround)
        {
            if (destroyOnCollision)
                Destroy(this.gameObject);
            else
                this.gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("BossReflector"))
        {
            gameObject.layer = 9;
            GetComponent<Rigidbody2D>().velocity = -direction * 10;
            transform.RotateAround(transform.position, Vector3.forward, 180);
        }
        else if(collision.gameObject.CompareTag("Arrow"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }

    public void DisableGroundCollision()
    {
        GetComponent<Rigidbody2D>().excludeLayers = groundLayers;
    }

    public void ShootInDirection(Vector3 direction, float speed, float duration)
    {
        this.direction = direction;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
}
