using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("State Locks")]
    public bool canLook = true;
    public bool canMove = true;

    [Header("Movement Settings")]
    public float walkSpeed = 8.0f;
    public float jumpHeight = 1.0f;
    public float gravity = -25f;

    [Header("Look Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    private float cameraPitch = 0f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 4f;
    public float crouchTransitionSpeed = 10f;

    public float standingCameraHeight = 0.8f;
    public float crouchCameraHeight = 0.2f;

    private bool isCrouching = false;

    private CharacterController controller;
    private float velocityY = 0f;
    private bool isGrounded;

    void Awake()
    {
        ServiceLocator.RegisterPlayerController(this);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canLook) MouseLook();
        HandleCrouch();
        if (canMove) Movement();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCenter = isCrouching ? -0.5f : 0f;
        float targetCameraY = isCrouching ? crouchCameraHeight : standingCameraHeight;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = new Vector3(0, Mathf.Lerp(controller.center.y, targetCenter, Time.deltaTime * crouchTransitionSpeed), 0);

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.localPosition = camPos;
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocityY < 0)
        {
            velocityY = -2f;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocityY * Time.deltaTime);
    }
}