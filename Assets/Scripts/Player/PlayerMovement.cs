using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAcceleration = 1.0f;


    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 1;
    [SerializeField] private float gravityScale = 1;


    [Header("Jump")]
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float maxCoyoteTimer = 0.5f;
    [SerializeField] private float jumpCutModifier = 0.5f;
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    [SerializeField] private float maxJumpBufferTimer = 0.2f;

    [Header("Wall Jump/Slide")]
    [SerializeField] bool isWallSliding = false;

    [SerializeField] bool isWallJumping = false;
    [SerializeField] bool isSpringJumping = false;
    [Space(10)]
    [SerializeField] float wallSlideSpeed = 2f;

    float wallJumpTimer = 0;
    float wallJumpDirection;
    [SerializeField] Vector2 wallJumpPower;
    [SerializeField] float maxWallJumpTimer = 0.2f;
    [SerializeField] float wallJumpDuration = 0.4f;

    [SerializeField] Transform wallCheckTransform;
    [SerializeField] LayerMask wallLayer;

    private float drag = 0.2f;

    private bool isInverse = false;
    private bool isBouncing = false;
    private bool canMove = true;
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private bool canWallSlide = true;
    [Space]
    [SerializeField] private LayerMask GroundLayerMask;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;
    Animator animator;

    private float horizontalInput;

    public float GravityScale { get => gravityScale; set => gravityScale = value; }
    public bool IsInverse { get => isInverse; set => isInverse = value; }
    public bool IsBouncing { get => isBouncing; set => isBouncing = value; }


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

        if (canMove)
        {
            #region Jumping
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpBufferTimer = maxJumpBufferTimer;

            }
            else
            {
                jumpBufferTimer -= Time.deltaTime;
            }

            if ((IsGrounded() || coyoteTimer < maxCoyoteTimer) & jumpBufferTimer > 0 && !isWallSliding)
            {
                if (!isInverse)
                {
                    animator.SetTrigger("Jump");
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
                }

                jumpBufferTimer = 0f;
            }

            //Variable Jump Height
            if ((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space)) && !IsBouncing)
            {
                if (!isInverse)
                    rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutModifier), ForceMode2D.Impulse);
                else
                    rb.AddForce(Vector2.up * rb.velocity.y * (1 - jumpCutModifier), ForceMode2D.Impulse);

            } 
            #endregion

            if (!IsGrounded())
            {
                animator.SetBool("IsGrounded", false);
                coyoteTimer += 1 * Time.deltaTime;

            }
            else
            {
                animator.SetBool("IsGrounded", true);
                IsBouncing = false;
                coyoteTimer = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            if(canWallSlide)
                WallSlide();
            if(canWallJump)
                WallJump();

            if (!isWallJumping)
            {
                Turn();
            } 
        }

        animator.SetFloat("XSpeed", Mathf.Clamp01(Mathf.Abs(horizontalInput)));
        animator.SetFloat("YSpeed", Mathf.Clamp(rb.velocity.y, -1.0f, 1.0f));

    }

    private void FixedUpdate()
    {

        if (canMove)
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

            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && coyoteTimer > maxCoyoteTimer)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }

            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - rb.velocity.x;

            //Calculate force along x-axis to apply to the player
            float movement = speedDif * accelRate;



            if (!isWallJumping && !isSpringJumping)
            {
                //Convert this to a vector and apply to rigidbody
                rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

                if (IsGrounded() && Mathf.Abs(horizontalInput) < 0.01f)
                {
                    float amount = (Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(drag))) * Mathf.Sign(rb.velocity.y);

                    rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
                }

            }

            #region Falling Gravity
            if (rb.velocity.y < 0 && !isInverse)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else if (rb.velocity.y > 0 && isInverse)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }
            #endregion 
        }
    }

    private void WallSlide()
    {
        if (IsPressedAgainstWall() && !IsGrounded() && horizontalInput != 0 && rb.velocity.y <= 0)
        {
            isWallSliding = true;
            animator.SetBool("IsWallSliding", true);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            if(IsGrounded())
            {
                wallJumpTimer = 0;
            }
            isWallSliding = false;
            animator.SetBool("IsWallSliding", false);

        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpTimer = maxWallJumpTimer;
            wallJumpDirection = -transform.localScale.x;

            CancelInvoke("StopWallJump");
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && wallJumpTimer > 0)
        {
            isWallJumping = true;
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(Mathf.Clamp(wallJumpDirection, -1.0f, 1.0f) * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            if(transform.localScale.x != wallJumpDirection)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }

            Invoke("StopWallJump", wallJumpDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    private void SpringJump(object sender, SpringEventArgs args)
    {
        if (isWallSliding || IsGrounded())
        {
            isSpringJumping = false;

            CancelInvoke("StopSpringJump");
        }

        isSpringJumping = true;
        rb.velocity = Vector2.zero;
        rb.velocity = args.springPower;

        Invoke("StopSpringJump", args.springDuration);
    }

    private void StopSpringJump()
    {
        isSpringJumping = false;
    }



    private void Turn()
    {
        if((horizontalInput > 0 && transform.localScale.x < 0) || (horizontalInput < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

    }

    private bool IsPressedAgainstWall()
    {
        Debug.DrawLine(wallCheckTransform.position, wallCheckTransform.position + ((Vector3.right * transform.localScale.x) * 0.2f));
        return Physics2D.Raycast(wallCheckTransform.position, Vector2.right * transform.localScale.x, 0.2f, wallLayer);
    }

    public bool IsGrounded()
    {
        float extraHeightTest = 0.05f;
        RaycastHit2D raycastHit;
        if (!isInverse)
        {
            raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, extraHeightTest, GroundLayerMask);
        }
        else
        {
            raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.up, extraHeightTest, GroundLayerMask);
        }
        return (raycastHit.collider != null && rb.velocity.y < 0.01f);
    }

    private void EnableMovement(object sender, PlayerMovementEventArgs EventArgs)
    {
        canMove = EventArgs.newPlayerMoveState;

        if(!canMove)
        {
            Vector3 velocity = rb.velocity;
            velocity.x = 0;
            rb.velocity = velocity;
        }
        //Debug.Log("New Movement State is " + EventArgs.newPlayerMoveState);
    }

    private void OnEnable()
    {
        PlayerEvents.MovementActive += EnableMovement;
        Spring.SpringTriggered += SpringJump;
    }

    private void OnDisable()
    {
        PlayerEvents.MovementActive -= EnableMovement;
        Spring.SpringTriggered -= SpringJump;
    }

}
