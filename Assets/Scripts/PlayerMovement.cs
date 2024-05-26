using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAcceleration = 1.0f;
    [Space]
    [SerializeField] private float fallGravityMultiplier = 1;
    [SerializeField] private float gravityScale = 1;
    [Space]
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float maxCoyoteTimer = 0.5f;
    [SerializeField] private float jumpCutModifier = 0.5f;
    private float coyoteTimer = 0f;

    private float drag = 0.2f;

    [SerializeField] private LayerMask GroundLayerMask;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;

    private float horizontalInput;


    public float GravityScale { get => gravityScale; set => gravityScale = value; }


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

        if((IsGrounded() || coyoteTimer < maxCoyoteTimer) & (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space))
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutModifier), ForceMode2D.Impulse);
        }

        if(!IsGrounded())
        {
            coyoteTimer += 1 * Time.deltaTime;

        }
        else
        {
            coyoteTimer = 0;
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
        return (raycastHit.collider != null && rb.velocity.y < 0.01f);
    }

}
