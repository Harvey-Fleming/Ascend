using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityReverterField : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(collision.GetComponent<PlayerMovement>().IsInverse)
            {
                GravityPower.StaticActivate();
            }
        }
    }
}
