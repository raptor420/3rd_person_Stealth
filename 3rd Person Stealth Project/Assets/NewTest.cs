using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTest : MonoBehaviour
{
    Vector3 InputDir;
   public  Transform Targetcube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        InputDir = input.normalized;
        //if (input.magnitude > 0.1)
        //{
        //    float targetRot = (Mathf.Atan2(InputDir.x, InputDir.y)) * Mathf.Rad2Deg ;
        //    transform.eulerAngles = targetRot * Vector3.up;
        //}


        var dir = (Targetcube.position - transform.position).normalized;
        
        float targetrot = (Mathf.Atan2(dir.x, dir.y)) * Mathf.Rad2Deg;  //z axis follows target
       // float targetrot = -90 + (Mathf.Atan2(dir.x, dir.z)) * Mathf.Rad2Deg;  // x axis follows target
        transform.eulerAngles = ( targetrot  )* Vector3.forward;
      //  transform.rotation = Quaternion.Euler(0,0 , targetrot +90 );
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, Targetcube.position);
    }
}
