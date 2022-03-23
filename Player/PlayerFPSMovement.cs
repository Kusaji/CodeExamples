using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerFPSMovement : NetworkBehaviour
{
    #region Variables

    [Header("States")]
    public bool isGrounded;

    [Header("Inputs")]
    public float verticalInput;
    public float horizontalInput;
    public float jumpInput;

    [Header("Vectors")]
    public Vector3 playerMousePosition;

    [Header("Player Movement Stats")]
    public float moveSpeed = 3f;
    public float maxMoveSpeed = 3.5f;
    public float jumpHeight = 50.0f;

    [Header("Realtime Physics Stats")]
    public float currentSpeed;
    public Vector3 currentForce;
    public GameObject groundObject;
    public float currentMaxMoveSpeed;
    public bool isJumping;
    public bool isJumpPadding;

    [Header("Components")]
    public Rigidbody characterRB;

    [Header("Options")]
    public bool hideMouse;
    public bool lockMouse;

    [SerializeField] Transform groundRay1;
    [SerializeField] Transform groundRay2;
    [SerializeField] Transform groundRay3;
    [SerializeField] Transform groundRay4;

    [SerializeField] bool ray1Grounded;
    [SerializeField] GameObject ray1CollidingWith;

    [SerializeField] bool ray2Grounded;
    [SerializeField] GameObject ray2CollidingWith;

    [SerializeField] bool ray3Grounded;
    [SerializeField] GameObject ray3CollidingWith;

    [SerializeField] bool ray4Grounded;
    [SerializeField] GameObject ray4CollidingWith;

    [SerializeField] bool isSpiderManning;

    [SerializeField] Animator playerAnim;

    #endregion

    //Unity && Mirror callbacks
    #region Unity Callbacks

    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
        currentMaxMoveSpeed = maxMoveSpeed;
        isSpiderManning = false;
        isJumpPadding = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; } //Simple check if local player.

        GroundChecker();

        if (!isJumpPadding)
        {
            //Player input based on frames, not physics updates.
            PlayerInput();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                PlayerJump();
                playerAnim.SetTrigger("Jump");
            }
        }

        if (isGrounded)
        {
            playerAnim.SetBool("Falling", false);
        } 
        else
        {
            playerAnim.SetBool("Falling", true);
        }

    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            currentSpeed = characterRB.velocity.magnitude;

            if (!isSpiderManning)
            {
                PlayerMovement();
            }

            if (currentSpeed < 5 && !isGrounded)
            {
                isSpiderManning = true;
            }
            else
            {
                isSpiderManning = false;
            }
        }
    }

    #endregion

    //Methods relating to Player input and animation triggers.
    #region Player Input && Animation
    /// <summary>
    /// Handles Players Inputs based on Unity's Input system.
    /// </summary>
    void PlayerInput()
    {
        //Set input floats / axis
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");


        playerAnim.SetFloat("inputY", verticalInput);
        playerAnim.SetFloat("inputX", horizontalInput);

        if (verticalInput == 0 && horizontalInput == 0 && !isJumping && isGrounded && characterRB.velocity.magnitude < 10f && !isJumpPadding)
        {
            characterRB.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Locks and Hides mouse based on options selected in Editor, useful for debug testing.
    /// </summary>
    void LockMouse()
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (hideMouse)
        {
            Cursor.visible = false;
        }
    }

    #endregion

    //Methods relating to player movement
    #region Player Movement

    /// <summary>
    /// Rigidbody movement which is clamped to maxSpeed variable.
    /// </summary>
    void PlayerMovement()
    {
        if (!isJumpPadding)
        {
            //Forward movement
            //Move Forward with calculated force to hit max speed.
            if (verticalInput > 0)
            {
                Vector3 targetVelocity = transform.rotation * Vector3.forward * currentMaxMoveSpeed;
                Vector3 force = (targetVelocity - characterRB.velocity) * moveSpeed;
                force.y = 0;
                currentForce = force;
                characterRB.AddForce(force);
            }
            //Move Backwards with calculated force to hit max speed.
            else if (verticalInput < 0)
            {
                Vector3 targetVelocity = transform.rotation * -Vector3.forward * currentMaxMoveSpeed;
                Vector3 force = (targetVelocity - characterRB.velocity) * moveSpeed;
                force.y = 0;
                currentForce = force;
                characterRB.AddForce(force);
            }

            //Horizontal Movement
            //Strafe right with calculated force to hit max speed.
            if (horizontalInput > 0)
            {
                Vector3 targetVelocity = transform.rotation * Vector3.right * currentMaxMoveSpeed;
                Vector3 force = (targetVelocity - characterRB.velocity) * moveSpeed;
                force.y = 0;
                currentForce = force;
                characterRB.AddForce(force);
            }
            //Strafe left with calculated force to hit max speed.
            else if (horizontalInput < 0)
            {
                Vector3 targetVelocity = transform.rotation * -Vector3.right * currentMaxMoveSpeed;
                Vector3 force = (targetVelocity - characterRB.velocity) * moveSpeed;
                force.y = 0;
                currentForce = force;
                characterRB.AddForce(force);
            }

            if (horizontalInput == 0 && verticalInput == 0 && jumpInput == 0 && isGrounded)
            {
                characterRB.velocity = new Vector3(0, -5, 0);
            }
        }
    }

    void PlayerJump()
    {
        characterRB.AddForce(0f, jumpHeight, 0f, ForceMode.Impulse);
        StartCoroutine(JumpCooldown());
    }

    void GroundChecker()
    {
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;

        if (Physics.Raycast(groundRay1.position, transform.TransformDirection(-Vector3.up), out hit1, 1f))
        {
            if (hit1.collider.gameObject != null && hit1.collider.gameObject.name != this.gameObject.name && hit1.collider.gameObject.CompareTag("Ground"))
            {
                ray1Grounded = true;
                ray1CollidingWith = hit1.collider.gameObject;
            }
        }
        else
        {
            ray1Grounded = false;
            ray1CollidingWith = null;
        }

        if (Physics.Raycast(groundRay2.position, transform.TransformDirection(-Vector3.up), out hit2, 1f))
        {
            if (hit2.collider.gameObject != null && hit2.collider.gameObject.name != this.gameObject.name && hit2.collider.gameObject.CompareTag("Ground"))
            {
                ray2Grounded = true;
                ray2CollidingWith = hit2.collider.gameObject;
            }
        }
        else
        {
            ray2Grounded = false;
            ray2CollidingWith = null;
        }

        if (Physics.Raycast(groundRay3.position, transform.TransformDirection(-Vector3.up), out hit3, 1f))
        {
            if (hit3.collider.gameObject != null && hit3.collider.gameObject.name != this.gameObject.name && hit3.collider.gameObject.CompareTag("Ground"))
            {
                ray3Grounded = true;
                ray3CollidingWith = hit3.collider.gameObject;
            }

        }
        else
        {
            ray3Grounded = false;
            ray3CollidingWith = null;
        }

        if (Physics.Raycast(groundRay4.position, transform.TransformDirection(-Vector3.up), out hit4, 1f))
        {
            if (hit4.collider.gameObject != null && hit4.collider.gameObject.name != this.gameObject.name && hit4.collider.gameObject.CompareTag("Ground"))
            {
                ray4Grounded = true;
                ray4CollidingWith = hit4.collider.gameObject;
            }
        }
        else
        {
            ray4Grounded = false;
            ray4CollidingWith = null;
        }

        if (!ray1Grounded && !ray2Grounded && !ray3Grounded && !ray4Grounded)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

    public void JumpPadInputBlock(float inputBlockTime)
    {
        StartCoroutine(JumpPadRoutine(inputBlockTime));
    }

    IEnumerator JumpPadRoutine(float inputBlockTime)
    {
        var oldMaxMoveSpeed = maxMoveSpeed;
        isJumpPadding = true;
        maxMoveSpeed = 9999;
        yield return new WaitForSeconds(inputBlockTime);
        maxMoveSpeed = oldMaxMoveSpeed;
        isJumpPadding = false;
    }

    IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }
    #endregion
}


/*if (Physics.Raycast(groundRay1.position, transform.TransformDirection(-Vector3.up), out hit4, 1f))
{
    if (hit4.collider.gameObject != null && hit4.collider.gameObject.name != this.gameObject.name && hit4.collider.gameObject.CompareTag("Ground"))
    {
        groundObject = hit4.collider.gameObject;
        ray4CollidingWith = groundObject;

        float distanceToGround = hit4.distance;
        if (distanceToGround >= 0.5)
        {
            ray4Grounded = false;
            ray4CollidingWith = null;
        }
        else
        {
            ray4Grounded = true;
        }
    }
    else
    {
        ray4CollidingWith = null; ;
    }
}*/