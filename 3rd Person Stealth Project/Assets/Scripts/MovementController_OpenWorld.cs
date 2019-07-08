using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController_OpenWorld : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed;
    [SerializeField]
    float AllowRotation = 0.1f;
    [SerializeField]
    float MovementSpeed =10;
    [SerializeField]
    float GravityMulitplier = 1;
    private float InputX, InputZ,Speed;
    float gravity;
    private Camera cam;
    
    private CharacterController characterController;

    private Vector3 desiredMoveDirection;
        void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
         InputX = Input.GetAxis("Horizontal");
       
        InputZ = Input.GetAxis("Vertical");
        InputDecider(); // for rotation
        MovementManager();
    }

    void InputDecider()
    {

        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        if (Speed > AllowRotation)
        {

            RotationManager(); // we wont go here if inputX,inputZ is zero


        }
        else {


            desiredMoveDirection = Vector3.zero; // we do this cause when inpuX,inputZ becomes zero is isnt updating desired direction to zero



        }




    }

    
    void RotationManager()
    {
       
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        desiredMoveDirection = forward * InputZ + right * InputX;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection),RotationSpeed);


    }

    void MovementManager()
    {
        gravity -= 9.8f * Time.deltaTime;
        gravity = gravity * GravityMulitplier;
        Vector3 movDirection = desiredMoveDirection * Time.deltaTime * MovementSpeed;
        movDirection = new Vector3(movDirection.x, gravity, movDirection.z);
        characterController.Move(movDirection);
        if (characterController.isGrounded)
        {

            gravity = 0;

        }

    }

  
    
}
