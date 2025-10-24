using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAnimation : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed;


    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 1;
    [SerializeField] private float gravityScale = 1;
    [SerializeField] private float maxFallSpeed = 15;
    [SerializeField] private bool isGravityEnabled = true;


    [SerializeField] private bool canMove = true;

    [Space]
    [SerializeField] private LayerMask GroundLayerMask;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;
    Animator animator;

    private float horizontalInput;

    public float GravityScale { get => gravityScale; set => gravityScale = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsGravityEnabled { get => isGravityEnabled; set => isGravityEnabled = value; }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (!IsGrounded())
        {
            animator.SetBool("IsGrounded", false);
        }
        else if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        //Animations for when dialogue is playing
        if (!IsGrounded())
        {
            animator.SetBool("IsGrounded", false);
        }

        animator.SetFloat("XSpeed", Mathf.Clamp01(Mathf.Abs(horizontalInput)));

        animator.SetFloat("YSpeed", -Mathf.Clamp(rb.velocity.y, -1.0f, 1.0f));
    }

    private void FixedUpdate()
    {
        #region Falling Gravity
        if (isGravityEnabled)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
            }
            else if (rb.velocity.y > 0)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, maxFallSpeed));
            }
            else
            {
                rb.gravityScale = gravityScale;
            }
        }
        else
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        #endregion
    }

    public bool IsGrounded()
    {
        float extraHeightTest = 0.05f;
        RaycastHit2D raycastHit;
        raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.up, extraHeightTest, GroundLayerMask);
        return raycastHit.collider != null && rb.velocity.y < 0.01f;
    }
}
