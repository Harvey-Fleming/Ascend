using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; 
            GetComponent<Rigidbody2D>().gravityScale = 9f; 
        }
        else if(collision.gameObject.name == "Wooden Bridge")
        {
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            collision.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
