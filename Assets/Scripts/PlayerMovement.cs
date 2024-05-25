using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAcceleration = 1.0f;
    [Space]
    [SerializeField] private Vector3 acceleration;
    [SerializeField] private Vector3 velocity;
    [Space]
    [SerializeField] private float fallGravityMultiplier = 1;
    [SerializeField] private float gravityScale = 1;
    [Space]
    [SerializeField] private float jumpForce = 1;

    private float drag = 0.2f;

    [SerializeField] private LayerMask GroundLayerMask;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;

    private float horizontalInput;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            Turn();
        }

        if(IsGrounded() & (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }


    }

    private void FixedUpdate()
    {

        //Calculate the desired speed based on input from the player
        float targetSpeed = horizontalInput * maxSpeed;

        float accelRate;

        //Calculate are run acceleration & deceleration forces using formula: amount = (50 * acceleration) / runMaxSpeed
        float targetaccel = (50 * speedAcceleration) / maxSpeed;

        //Calculate whether we should be accelerating or decelerating
        if (IsGrounded())
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? targetaccel : targetaccel;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? targetaccel * 0.5f : targetaccel * 0.5f;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;

        //Calculate force along x-axis to apply to the player
        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if(IsGrounded() && Mathf.Abs(horizontalInput) < 0.01f)
        {
            float amount = (Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(drag))) * Mathf.Sign(rb.velocity.y);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        Debug.Log("Velocity is " + rb.velocity);
        #region Falling Gravity
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        #endregion
    }

    private void Turn()
    {
        if((horizontalInput > 0 && transform.localScale.x < 0)|| (horizontalInput < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

    }

    public bool IsGrounded()
    {
        float extraHeightTest = 0.05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, extraHeightTest, GroundLayerMask);
        return raycastHit.collider != null;
    }

}
