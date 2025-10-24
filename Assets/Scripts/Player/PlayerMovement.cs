using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAcceleration = 1.0f;

    Vector3 savedDebugPos;


    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 1;
    [SerializeField] private float gravityScale = 1;
    [SerializeField] private float maxFallSpeed = 15;
    [SerializeField] private bool isGravityEnabled = true;
    private float fallSpeedCameraDampThreshold = -15f;


    [Header("Jump")]
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float maxCoyoteTimer = 0.5f;
    [SerializeField] private float jumpCutModifier = 0.5f;
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    [SerializeField] private float maxJumpBufferTimer = 0.2f;
    [SerializeField] private int airJumpCounter = 1;

    

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
    [SerializeField] private bool canMove = true;

    [Header("Ability States")]
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private bool canWallSlide = true;
    [SerializeField] private bool canDoubleJump = false;

    [Space]
    [SerializeField] private LayerMask GroundLayerMask;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;
    Animator animator;

    private float horizontalInput;

    public float GravityScale { get => gravityScale; set => gravityScale = value; }
    public bool IsInverse { get => isInverse; set => isInverse = value; }
    public bool IsBouncing { get => isBouncing; set => isBouncing = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsGravityEnabled { get => isGravityEnabled; set => isGravityEnabled = value; }


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        fallSpeedCameraDampThreshold = CameraManager.instance.fallSpeedYDampChangeThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.L))
        {
            savedDebugPos = transform.position;
        }
        else if(Input.GetKeyDown(KeyCode.K))
        {
            transform.position = savedDebugPos;
        }

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
                    animator.SetTrigger("Jump");
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
                }

                jumpBufferTimer = 0f;
            }

            //Variable Jump Height
            if ((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space)) && !IsBouncing)
            {
                rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutModifier), ForceMode2D.Impulse);
            }  
            #endregion

            if (!IsGrounded())
            {
                animator.SetBool("IsGrounded", false);
                coyoteTimer += 1 * Time.deltaTime;

                if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && coyoteTimer >= maxCoyoteTimer && airJumpCounter > 0 && canDoubleJump)
                {
                    AirJump();
                }
            }
            else if(IsGrounded())
            {
                animator.SetBool("IsGrounded", true);
                IsBouncing = false;
                coyoteTimer = 0;
                airJumpCounter = 1;
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
        else
        {
            //Animations for when dialogue is playing
            if(!IsGrounded())
            {
                animator.SetBool("IsGrounded", false);
            }
        }

        //if velocity is lower? than the threshold and not already looking ahead
        if(rb.velocity.y < fallSpeedCameraDampThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        if(rb.velocity.y >= fallSpeedCameraDampThreshold && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }

        animator.SetFloat("XSpeed", Mathf.Clamp01(Mathf.Abs(horizontalInput)));

        if(!isInverse)
            animator.SetFloat("YSpeed", Mathf.Clamp(rb.velocity.y, -1.0f, 1.0f));
        else
            animator.SetFloat("YSpeed", -Mathf.Clamp(rb.velocity.y, -1.0f, 1.0f));

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
            if (isGravityEnabled)
            {
                if (rb.velocity.y < 0 && !isInverse)
                {
                    rb.gravityScale = gravityScale * fallGravityMultiplier;
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
                }
                else if (rb.velocity.y > 0 && isInverse)
                {
                    rb.gravityScale = gravityScale * fallGravityMultiplier;
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, maxFallSpeed));
                }
                else
                {
                    if (IsInverse)
                    {
                        rb.gravityScale = -gravityScale;
                    }
                    {
                        rb.gravityScale = gravityScale;
                    }
                } 
            }
            else
            {
                rb.gravityScale = 0;
            }
            #endregion 
        }
    }

    private void AirJump()
    {
        airJumpCounter = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        if (IsInverse)
        {
            rb.AddForce(Vector2.down * (jumpForce * 0.75f), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.up * (jumpForce * 0.75f), ForceMode2D.Impulse);
        }
        IsBouncing = true;
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
        return raycastHit.collider != null && (isInverse ? (rb.velocity.y > -0.01f && rb.velocity.y < 0.01f) : rb.velocity.y < 0.01f);
    }

    public void FlipGraivty()
    {
        IsInverse = !IsInverse;
        GravityScale *= -1;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        //gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x,gameObject.transform.localScale.y * -1, gameObject.transform.localScale.z);
        //GetComponent<SpriteRenderer>().flipY = !GetComponent<SpriteRenderer>().flipY;
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        transform.Rotate(new Vector3(0,0,180));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("GravityFlip"))
        {
            FlipGraivty();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GravityFlip") && IsInverse)
        {
            FlipGraivty();
        }
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

    public void EnableMovement(bool canMove)
    {
        this.canMove = canMove;

        if (!canMove)
        {
            Vector3 velocity = rb.velocity;
            velocity.x = 0;
            rb.velocity = velocity;
        }
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
