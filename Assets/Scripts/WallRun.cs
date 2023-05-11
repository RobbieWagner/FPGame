using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRun : MonoBehaviour
{

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallClimbSpeed;
    [SerializeField] private float maxWallRunTime;
    private float wallRunTimer;

    private float horizontal;
    private float vertical;

    [SerializeField] private float wallcheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [SerializeField] private Transform bodyT;
    private PlayerController pController;
    private Rigidbody rb;
    [SerializeField] private CharacterController controller;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckForRunableWalls();
        StateMachine();
    }
    
    private void FixedUpdate() 
    {
        if(pController.isWallRunning)
            WallRunningMovement();
    }

    private void CheckForRunableWalls()
    {
        wallRight = Physics.Raycast(transform.position, bodyT.right, out rightWallhit, wallcheckDistance, wallLayer);
        wallLeft = Physics.Raycast(transform.position, -bodyT.right, out leftWallhit, wallcheckDistance, wallLayer);
    }

    private void StateMachine()
    {
        Vector2 move = pController.move;

        horizontal = move.x;
        vertical = move.y;

        if((wallLeft || wallRight) && vertical > 0 && !pController.isGrounded)
        {
            if(!pController.isWallRunning && pController.canWallRun)
            StartWallRun();
        }

        else if(pController.isWallRunning) StopWallRun();
    }

    private void StartWallRun()
    {
        pController.isWallRunning = true;
    }

    public void WallRunningMovement()
    {
        pController.useGravity = false;

        pController.velocity.y = wallClimbSpeed;
        Vector3 movement = new Vector3(0,0,0);

        if(pController.isRunning) movement = new Vector3(0, wallClimbSpeed, 0) + (pController.move.y * transform.forward) + (pController.move.x * transform.right);
        else movement = new Vector3(0, -wallClimbSpeed, 0) + (pController.move.y * transform.forward) + (pController.move.x * transform.right);
        
        controller.Move(movement * pController.moveSpeed * Time.deltaTime);
    }

    private void StopWallRun()
    {
        pController.isWallRunning = false;
        pController.useGravity = true;
        pController.canWallRun = false;
    }
}
