using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;

public class thirdPersonMovement : MonoBehaviour
{
//  //-------=!=-------\\
    #region Necessary Variables |

    public CharacterController controller;
    public Transform cam;

    public float speed = 4f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    #endregion
    //  \\-------=!=-------//

    // animation
    public Animator playerAnimator;
    public bool walking;
    public bool walkingBackwards;

    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackwards = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    public float jumpStrength = 5f;

    public float run_speed = 8f;
    public float walk_speed = 4f;

    public KeyCode lockMouse = KeyCode.R;
    public bool mouseLock;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public Vector3 velocity;

    public void Jump()
    {
        velocity.y = jumpStrength;
        // Debug.Log("Delay!");
    }

    void Update()
    {

        if (Input.GetKey(moveForward) || Input.GetKey(moveLeft) || Input.GetKey(moveRight) || Input.GetKey(moveBackwards))
        {
            playerAnimator.SetTrigger("jog");
            playerAnimator.ResetTrigger("idle");
            walking = true;
        }

        if (!Input.GetKey(moveForward) && !Input.GetKey(moveLeft) && !Input.GetKey(moveRight) && !Input.GetKey(moveBackwards))
        {
            playerAnimator.SetTrigger("idle"); 
            playerAnimator.ResetTrigger("jog");
            walking = false;
        }

        //Sprint animations
        if (Input.GetKey(sprintKey) && !walkingBackwards && walking)
        {
            speed = run_speed;
            playerAnimator.SetTrigger("sprint");
            playerAnimator.ResetTrigger("jog");
        }
        if (!Input.GetKey(sprintKey) && (Input.GetKey(moveForward) || Input.GetKey(moveLeft) || Input.GetKey(moveRight) || Input.GetKey(moveBackwards)))
        {
            speed = walk_speed;
            playerAnimator.ResetTrigger("sprint");
            playerAnimator.SetTrigger("jog");
        }


        #region Gravity

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            playerAnimator.ResetTrigger("jump");

            velocity.y = -2f;
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            playerAnimator.SetTrigger("jump");
            
            Invoke("Jump", 0.45f);
        }

        velocity.y += gravity * Time.deltaTime;

        #endregion


//      //-------==-------\\
        #region  |Necessary movement|

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        controller.Move(velocity * Time.deltaTime);

        #endregion
//      \\-------==-------//

        if (Input.GetKeyDown(lockMouse))
        {
            mouseLock = !mouseLock;
            if (mouseLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundDistance);
    }
}
