using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 1.8f;
    public float runSpeed = 4.5f;
    public float rotationSpeed = 12f;
    public float jumpHeight = 0.6f;
    public float gravity = -9.81f;

    [Header("Joystick Bağlantısı")]
    public FixedJoystick joystick;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private float jumpGraceTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (jumpGraceTimer > 0f)
        {
            jumpGraceTimer -= Time.deltaTime;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f && jumpGraceTimer <= 0f)
        {
            velocity.y = -2f;
        }

        // Klavye ve Joystick girişlerini oku
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (joystick != null)
        {
            if (Mathf.Abs(joystick.Horizontal) > 0.1f) horizontal = joystick.Horizontal;
            if (Mathf.Abs(joystick.Vertical) > 0.1f) vertical = joystick.Vertical;
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float inputMagnitude = Mathf.Clamp01(new Vector2(horizontal, vertical).magnitude);

        bool isRunningInput = Input.GetKey(KeyCode.LeftShift) || inputMagnitude > 0.85f;
        bool isMoving = inputMagnitude >= 0.1f;

        // Zıplama kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpGraceTimer <= 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpGraceTimer = 0.25f;
            SetAnimStates(false, false, false, true);
        }
        else if (isGrounded && jumpGraceTimer <= 0f)
        {
            if (!isMoving)
            {
                SetAnimStates(true, false, false, false);
            }
            else if (isRunningInput)
            {
                SetAnimStates(false, false, true, false);
            }
            else
            {
                SetAnimStates(false, true, false, false);
            }
        }

        // Karakteri yönlendir ve ilerlet
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float currentSpeed = isRunningInput ? runSpeed : walkSpeed;
            controller.Move(direction * currentSpeed * Time.deltaTime);
        }

        // Yerçekimi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void SetAnimStates(bool idle, bool walking, bool running, bool jumping)
    {
        animator.SetBool("isIdle", idle);
        animator.SetBool("isWalking", walking);
        animator.SetBool("isRunning", running);
        animator.SetBool("isJumping", jumping);
    }
}