using UnityEngine;
using Cinemachine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Camera Settings")]    
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraVerticalLimit = 80f;
    [SerializeField] private float cameraSprintNoise = 1.5f;
    [SerializeField] private float cameraJumpNoise = 3f;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 15f;    
    [SerializeField] private float slideDuration = 1f;  
    [SerializeField] private float slideHeight = 0.5f;  
    [SerializeField] private float slideCameraOffset = 0.5f; 
    [SerializeField] private float slideTransitionTime = 0.2f;

    [Header("Bunnyhop Settings")]
    [SerializeField] private float bunnyHopMultiplier = 1.2f;
    [SerializeField] private float airControl = 0.5f;    

    [Header("WallJump Settings")]
    [SerializeField] private float wallCheckDistance = 0.6f;    
    [SerializeField] private float wallJumpVerticalForce = 15f; 
    [SerializeField] private float wallJumpHorizontalForce = 10f;
    [SerializeField] private float wallJumpCooldown = 0.5f;     
    [SerializeField] private LayerMask wallMask;

    [HideInInspector] public bool IsRunning { get; private set; }

    private CharacterController controller;
    private Vector3 moveDirection;
    private float rotationX;    
    private bool isDashing;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private Vector3 velocity;
    private float originalHeight;       
    private float originalCameraY;      
    private bool isSliding = false;
    private Vector3 savedVelocity;      
    private bool isJumping = false;
    private bool isWallJumping;
    private float lastWallJumpTime;
    private Vector3 wallNormal;    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        velocity = Vector3.zero;
        originalHeight = controller.height;
        originalCameraY = virtualCamera.transform.localPosition.y;        
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleDash();
        HandleSlide();
        HandleWallJump();
    }

    private void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        IsRunning = Input.GetKey(KeyCode.LeftShift);        

        Vector3 horizontalMove = (forward * vertical + right * horizontal) * curSpeed;

        if (controller.isGrounded)
        {
            if (isJumping)
            {
                savedVelocity = Vector3.zero;
                isJumping = false;
            }

            velocity.y = -0.1f;

            if (Input.GetButton("Jump"))
            {
                savedVelocity = horizontalMove * bunnyHopMultiplier;
                velocity.y = jumpForce;
                isJumping = true;
                StartCoroutine(JumpCameraShake());
            }
        }
        else
        {
            if (savedVelocity.magnitude > 0)
            {
                horizontalMove = Vector3.Lerp(horizontalMove, savedVelocity, airControl);
            }

            velocity.y -= gravity * Time.deltaTime;
        }

        Vector3 finalMove = horizontalMove + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private IEnumerator JumpCameraShake()
    {
        cameraNoise.m_AmplitudeGain = cameraJumpNoise;
        yield return new WaitForSeconds(0.1f);
        cameraNoise.m_AmplitudeGain = 0;
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -cameraVerticalLimit, cameraVerticalLimit);

        virtualCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, mouseX, 0);
        
        if (!isJumping)
        {
            cameraNoise.m_AmplitudeGain = Input.GetKey(KeyCode.LeftShift) ? cameraSprintNoise : 0f;
        }        
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        float originalSpeed = walkSpeed;
        walkSpeed = dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        walkSpeed = originalSpeed;
        isDashing = false;
    }

    private void HandleSlide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && controller.isGrounded)
        {
            StartCoroutine(PerformSlide());
        }
    }

    private IEnumerator PerformSlide()
    {
        isSliding = true;
        Vector3 slideDirection = transform.forward;

        yield return StartCoroutine(ChangeHeight(slideHeight, slideCameraOffset, slideTransitionTime));

        while (isSliding)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                slideDirection = (transform.forward * Input.GetAxis("Vertical") +
                                  transform.right * Input.GetAxis("Horizontal")).normalized;
            }

            controller.Move(slideDirection * slideSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") || Input.GetKeyUp(KeyCode.LeftControl))
            {
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(ChangeHeight(originalHeight, originalCameraY, slideTransitionTime));
        isSliding = false;
    }

    private IEnumerator ChangeHeight(float targetHeight, float targetCameraY, float time)
    {
        float initialHeight = controller.height;
        float initialCameraY = virtualCamera.transform.localPosition.y;
        float elapsed = 0;

        while (elapsed < time)
        {
            controller.height = Mathf.Lerp(initialHeight, targetHeight, elapsed / time);
            controller.center = new Vector3(0, controller.height / 2, 0); 

            Vector3 camPos = virtualCamera.transform.localPosition;
            camPos.y = Mathf.Lerp(initialCameraY, targetCameraY, elapsed / time);
            virtualCamera.transform.localPosition = camPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = new Vector3(0, targetHeight / 2, 0);
        virtualCamera.transform.localPosition = new Vector3(0, targetCameraY, 0);
    }

    private void HandleWallJump()
    {
        if (Input.GetButtonDown("Jump") && CanWallJump())
        {
            PerformWallJump();
        }
    }

    private bool CanWallJump()
    {        
        return !controller.isGrounded &&
               CheckWalls() &&
               Time.time > lastWallJumpTime + wallJumpCooldown;
    }

    private bool CheckWalls()
    {
        RaycastHit hit;
        bool wallDetected = false;
        
        // TODO: Заменить на checkSphere
        Vector3[] directions = new Vector3[]
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            new Vector3(transform.forward.x, 0, transform.forward.z),
            new Vector3(-transform.forward.x, 0, transform.forward.z),
            new Vector3(transform.forward.x, 0, -transform.forward.z),
            new Vector3(-transform.forward.x, 0, -transform.forward.z),
        };

        foreach (Vector3 dir in directions)
        {            
            if (Physics.Raycast(transform.position, dir, out hit, wallCheckDistance, wallMask))
            {
                wallNormal = hit.normal;
                wallDetected = true;
                break;
            }
        }

        return wallDetected;
    }

    private void PerformWallJump()
    {     
        isWallJumping = true;
        lastWallJumpTime = Time.time;
        
        Vector3 jumpDirection = (Vector3.up + wallNormal).normalized;

        velocity = Vector3.zero;

        velocity.y = wallJumpVerticalForce;
        velocity += jumpDirection * wallJumpHorizontalForce;

        StartCoroutine(JumpCameraShake());
        StartCoroutine(ResetWallJump());
    }

    private IEnumerator ResetWallJump()
    {
        yield return new WaitForSeconds(0.2f);
        isWallJumping = false;
    }
}