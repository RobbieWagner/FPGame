using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput controls;
    public float moveSpeed = 6f;
    public Vector3 velocity;
    private float gravity = -9.8f;
    [SerializeField] private float gravityFactor = 1f;

    [HideInInspector] public Vector2 move;
    private float jumpHeight = 2.4f;
    private CharacterController controller;

    public Transform ground;

    public float distanceToGround = .5f;

    public LayerMask groundMask;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool canWallRun;
    [HideInInspector] public bool isWallRunning;
    [HideInInspector] public bool useGravity;

    [SerializeField] private WallRun wallRun;

    private void Awake() 
    {
        controls = new PlayerInput();
        controller = GetComponent<CharacterController>();

        isRunning = false;
        isWallRunning = false;
        useGravity = true;

        controls.Player.Run.started += CollectionExtensions => StartRun();
        controls.Player.Run.canceled += CollectionExtensions => StopRun();
    }

    // Update is called once per frame
    void Update()
    {
        Grav();
        PlayerMovement();
        Jump();
    }

    private void Grav()
    {
        if(useGravity)
        {
            isGrounded = Physics.CheckSphere(ground.position, distanceToGround, groundMask);
            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                canWallRun = true;
            }

            velocity.y += gravity * Time.deltaTime * gravityFactor;
            controller.Move(velocity * Time.deltaTime);
        }
        
    }

    private void PlayerMovement()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();

        if(!isWallRunning)
        {
            Vector3 movement = (move.y * transform.forward) + (move.x * transform.right);
            controller.Move(movement * moveSpeed * Time.deltaTime);
        }
        else if(wallRun != null) 
        {
            wallRun.WallRunningMovement();
        }
    }

    private void Jump()
    {
        if(controls.Player.Jump.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void StartRun()
    {
        if(!isRunning)
        {
            isRunning = true;
            moveSpeed *= 2;
        }
    }
    
    private void StopRun()
    {
        if(isRunning)
        {
            isRunning = false;
            moveSpeed /= 2;
        }
    }

    private void OnEnable() 
    {
        controls.Enable();
    }

    private void OnDisable() 
    {
        controls.Disable();
    }
}
