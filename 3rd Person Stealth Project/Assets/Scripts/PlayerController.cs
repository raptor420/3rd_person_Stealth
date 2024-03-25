using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
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
    private  bool inFireArmStance;
    bool Fire;
    public Vector2 inputDir;
    [SerializeField]
    bool AllowPlayertoMove;
    float CurrentSpeed;
    float SmoothVelocity;
    public float GravityMultiplier;
    float gravity;
    public bool InBox;
    public Transform BoxTransform;
    Animator BoxAnim;
    PlayerInput playerInput;
  //  public static event Action<bool> OnFireState;
     
    // Start is called before the first frame update
    void Awake()
    {
        PlayerObject = Anim.gameObject;
        BoxAnim = BoxTransform.GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        InBox= false;
        BoxTransform.gameObject.SetActive(false);
        playerInput = GetComponent<PlayerInput>();  
    }
    private void OnEnable()
    {
        PlayerInput.onSpacePressed += PlayerInput_onSpacePressed;
        PlayerInput.onReleaseClick += PlayerInput_onReleaseClick;
    }
    

    private void PlayerInput_onReleaseClick()
    {
        if (!inFireArmStance) return;
        Firing();
    }

    private void PlayerInput_onSpacePressed()
    {
      BoxControlToggle();   
    }

    private void OnDisable()
    {
        PlayerInput.onSpacePressed -= PlayerInput_onSpacePressed;
        PlayerInput.onReleaseClick -= PlayerInput_onReleaseClick;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = playerInput.inputDir;
        inputDir = input.normalized;

        TurnSmooth = inFireArmStance ? .3f : 0.05f;
        if (inputDir != Vector2.zero) // we can always move tho even if were not in stance;
        {
            RotationManager();
        }

        if (!inFireArmStance && !Fire)
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

        //FireInput
        if (!InBox)
        {
            AllowFireInput();
        }

        float targetspeed = inputDir.magnitude * RunSpeed;
        CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, targetspeed, ref SmoothVelocity, smoothspeedtime);

        // passing animation  values
        if (!InBox)
        {
            Anim.SetFloat("SpeedValue", inputDir.magnitude * 1, smoothspeedtime, Time.deltaTime);
            Anim.SetBool("IsFiring", inFireArmStance);
        }

        BoxAnimChecker();
    }

    private void BoxControlToggle()
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

    private void BoxAnimChecker()
    {
        if (InBox)
        {
            BoxTransform.gameObject.SetActive(true);
            PlayerObject.SetActive(false);
            BoxAnim.SetFloat("Speed", playerInput.inputDir.magnitude * RunSpeed);
        }
        else
        {
            BoxTransform.gameObject.SetActive(false);
            PlayerObject.SetActive(true);

        }
    }

    void AllowFireInput()
    {
        if (playerInput.isClicking && characterController.isGrounded)
        {
            inFireArmStance = true;
            // get in stance, disable the move 
            FireStance();

        }

      /*  else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Firing();
        }*/
        else
        {
            inFireArmStance = false;
        }
    }

    private void Firing()
    {
        inFireArmStance = false;
        //fire
        StartCoroutine(FireGun());
    }
    public bool GetIsInFireArmStance()
    {
        
        return inFireArmStance;
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
        float targetRot = (Mathf.Atan2(inputDir.x, inputDir.y)) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, TurnSmooth);

    }

    public void FireStance()
    {

        AllowPlayertoMove = false;
        Anim.SetBool("IsFiring",inFireArmStance);

    }
    IEnumerator FireGun()
    {
        AllowPlayertoMove = false;
        Fire = true;
        Anim.SetBool("Fire", Fire);
        yield return new WaitForSeconds(.4f);
      //  GetComponent<ShooterController>().Shoot
        Fire = false;
        Anim.SetBool("Fire", Fire);
        AllowPlayertoMove = true;

    }
}

