using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainRocks : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            DropRocks();
        }
    }

    private void DropRocks()
    {
        GetComponent<Animator>().SetTrigger("OnRockFall");
    }

}
