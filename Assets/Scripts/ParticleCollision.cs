using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if(other.tag == "Player")
        {
            FindObjectOfType<PlayerHealth>().TakeDamage();
        }
    }
}
