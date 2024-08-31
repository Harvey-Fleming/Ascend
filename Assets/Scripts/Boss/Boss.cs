using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    protected float maxHealth = 3;
    protected float currentHealth = 3;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage()
    {
        currentHealth--;
    }
}
