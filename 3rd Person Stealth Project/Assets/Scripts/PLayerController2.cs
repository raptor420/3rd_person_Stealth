using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PLayerController2 : MonoBehaviour
{
    /// <summary>
    ///  tobedone firearm draw 
    /// </summary>
    public float walkespeed;
    public float RunSpeed;
    public float TurnSmooth =0.1f;
    float turnSmoothVelocity;
  public    float smoothspeedtime = 0.1f;
    CharacterController characterController;
    public Animator Anim;
    GameObject PlayerObject;
    [SerializeField]
    private  bool InFireArmStance;
    bool Fire;
    public Vector2 InputDir;
    [SerializeField]
    bool AllowPlayertoMove;
    float CurrentSpeed;
    float SmoothVelocity;
    public float GravityMultiplier;
    float gravity;
    public bool InBox;
    public Transform BoxTransform;
    Animator BoxAnim;
  //  public static event Action<bool> OnFireState;
     
    // Start is called before the first frame update
    void Awake()
    {
        PlayerObject = Anim.gameObject;
        BoxAnim = BoxTransform.GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        InBox= false;
        BoxTransform.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        InputDir = input.normalized;

        TurnSmooth = InFireArmStance ? .3f  : 0.05f;
        if (InputDir != Vector2.zero) // we can always move tho even if were not in stance;
        {
            RotationManager();
        }

            if ( !InFireArmStance && !Fire)
            {
            AllowPlayertoMove = true;
                MoveCharacter();
            }

            else
        {
            AllowPlayertoMove = false;

        }
            // reseting g
        if (characterController.isGrounded)
        {

            gravity = 0;

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!InBox)
            {
                InBox = true;

            }
            else
            {
                InBox = false;
            }
        }
        if (InBox)
        {
            BoxTransform.gameObject.SetActive(true);
            PlayerObject.SetActive(false);
            BoxAnim.SetFloat("Speed", InputDir.magnitude * RunSpeed);
        }
        else
        {
            BoxTransform.gameObject.SetActive(false);
            PlayerObject.SetActive(true);

        }

        //FireInput
        if (!InBox)
        {
            AllowFireInput();
        }

        float targetspeed = InputDir.magnitude * RunSpeed;
        CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, targetspeed, ref SmoothVelocity, smoothspeedtime );

        // passing animation  values
        if (!InBox)
        {
            Anim.SetFloat("SpeedValue", InputDir.magnitude * 1, smoothspeedtime, Time.deltaTime);
            Anim.SetBool("IsFiring", InFireArmStance);
        }
    }
    void AllowFireInput()
    {
        if (Input.GetKey(KeyCode.Mouse0) && characterController.isGrounded)
        {
            InFireArmStance = true;
            // get in stance, disable the move 
            FireStance();

        }

        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            InFireArmStance = false;
            //fire
            StartCoroutine(FireGun());
        }
        else
        {
            InFireArmStance = false;
        }
    }
   
        
    void MoveCharacter()
    {

        if (AllowPlayertoMove)
        {
            gravity -= 9.8f * Time.deltaTime * GravityMultiplier;
            Vector3 velocity = transform.forward * CurrentSpeed + Vector3.up * gravity; 
            characterController.Move(velocity * Time.deltaTime);
        }
       

    }

    void RotationManager()
    {
        float targetRot = (Mathf.Atan2(InputDir.x, InputDir.y)) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, TurnSmooth);

    }

    public void FireStance()
    {

        AllowPlayertoMove = false;
        Anim.SetBool("IsFiring",InFireArmStance);

    }
    IEnumerator FireGun()
    {
        AllowPlayertoMove = false;
        Fire = true;
        Anim.SetBool("Fire", Fire);
        yield return new WaitForSeconds(.4f);
        Debug.Log("PEW");

        Fire = false;
        Anim.SetBool("Fire", Fire);
        AllowPlayertoMove = true;

    }
}

