using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    [SerializeField] private bool canHitGround = true;

    [SerializeField] private LayerMask groundLayers;

    public bool CanHitGround { get => canHitGround; set => canHitGround = value; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") && canHitGround)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void DisableGroundCollision()
    {
        GetComponent<Rigidbody2D>().excludeLayers = groundLayers;
    }

    public void ShootInDirection(Vector3 direction, float speed, float duration)
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
}
