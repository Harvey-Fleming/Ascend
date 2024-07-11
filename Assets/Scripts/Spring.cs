using System;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public static event EventHandler<SpringEventArgs> SpringTriggered;

    [SerializeField] private Vector2 springPower;
    [SerializeField] private float springDuration;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnSpringTriggered()
    {
        SpringTriggered?.Invoke(this, new SpringEventArgs(springPower, springDuration));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("OnSpringTrigger");
            OnSpringTriggered();
        }
    }
}

public class SpringEventArgs
{
    public Vector2 springPower;
    public float springDuration;

    public SpringEventArgs(Vector2 springPower, float springDuration)
    {
        this.springPower = springPower;
        this.springDuration = springDuration;
    }
}
