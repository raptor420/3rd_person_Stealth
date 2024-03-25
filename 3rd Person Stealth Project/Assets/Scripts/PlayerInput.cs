using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
   public Vector2  inputDir = Vector2.zero;
   public bool isClicking = false;
    public static event Action onSpacePressed;
    public static event Action onReleaseClick;
    private void Update()
    {
        inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isClicking = Input.GetKey(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton15))
        {
            onSpacePressed?.Invoke();

        }

        if ((Input.GetKeyUp(KeyCode.Mouse0))|| Input.GetKeyDown(KeyCode.JoystickButton11))
        {

            onReleaseClick?.Invoke();
        }


    }
}
