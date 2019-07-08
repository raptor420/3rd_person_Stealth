using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private Vector2 InputDir;
  
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        InputDir = input.normalized;
        float targetangle = (Mathf.Atan2(InputDir.x, InputDir.y)) * Mathf.Deg2Rad;
        transform.localEulerAngles = targetangle * Vector3.up;
    }
}
