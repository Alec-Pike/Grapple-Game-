using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

/*
    Movement controller based on a tutorial by Dave / GameDevelopment on YouTube
    Link: https://youtu.be/f473C43s8nE?si=A7cJH3LryJPt1ojU
*/
public class DavesMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float swingSpeed;
    private float moveSpeed;
    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce, jumpCooldown, airControl, maxYVelocity;
    private bool readyToJump = true;

    //[Header("Keybinds")]
    //[SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDepth, groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool drawGroundCheckGizmo;
    private bool grounded;

    [SerializeField] private Transform orientation;

    private float horizInput;
    private float vertInput;
    private Vector3 moveDir;
    private Rigidbody rb;

    [SerializeField] private Animator animator;
    private int jumpTriggerHash, groundedHash, moveSpeedHash, startSwingingHash, isSwingingHash;

    private bool isSwinging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        jumpTriggerHash = Animator.StringToHash("jumpTrigger");
        groundedHash = Animator.StringToHash("grounded");
        moveSpeedHash = Animator.StringToHash("moveSpeed");
        startSwingingHash = Animator.StringToHash("startSwinging");
        isSwingingHash = Animator.StringToHash("isSwinging");
    }

    // Update is called once per frame
    private void Update()
    {
        animator.SetBool(isSwingingHash, isSwinging);
        animator.ResetTrigger(startSwingingHash);
        if (isSwinging)
        {
            moveSpeed = swingSpeed;
        }
        else
        {


            moveSpeed = runSpeed;

            // ground check
            grounded = Physics.SphereCast(transform.position, groundCheckRadius, Vector3.down, out _, groundCheckDepth, whatIsGround);
            //Debug.Log("grounded: " + grounded);
            animator.SetBool(groundedHash, grounded);

            MyInput();
            SpeedControl();

            // handle drag
            if (grounded) rb.linearDamping = groundDrag;
            else rb.linearDamping = 0f;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetAxisRaw("Jump") > 0.1f && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (isSwinging) return;

        // calculate movement direction
        moveDir = orientation.forward * vertInput + orientation.right * horizInput;
        moveDir = moveDir.normalized; 
        animator.SetFloat(moveSpeedHash, moveDir.magnitude);

        // on ground
        if (grounded) rb.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);
        // in air
        else rb.AddForce(moveDir * moveSpeed * 10f * airControl, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit veloctiy if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        // limit Y velocity
        if (!isSwinging && rb.linearVelocity.y > maxYVelocity)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYVelocity, rb.linearVelocity.z);
        }
    }

    private void Jump()
    {
        animator.SetTrigger(jumpTriggerHash);

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // draw ground check visual
        if (drawGroundCheckGizmo)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3.down * groundCheckDepth), groundCheckRadius);
        }
    }

    public void SetSwinging(bool swinging)
    {
        isSwinging = swinging;

        animator.SetTrigger(startSwingingHash);
    }
}
