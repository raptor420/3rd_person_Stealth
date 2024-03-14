using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShooterController : MonoBehaviour
{
    PlayerController playerController;
    public Transform GunTip;
    bool ShootingStance;
    public Animator PlayerAnim;
    RaycastHit hit;
    public float PlayerVisionRadius = 5f;
    public LayerMask Targetmask;
    [SerializeField]
    Transform nearestTarget;
    [SerializeField]
    Transform lockedTarget;
    //Transform[] TargetsInAngle;
    public float ViewingAngle;
    [SerializeField]
    List<Transform> TargetsWithinAngle; 
    public LineRenderer GunLine;
    void Start()
    {
        TargetsWithinAngle = new List<Transform>();
        GunLine = GunTip.GetComponent<LineRenderer>();
        playerController = GetComponent<PlayerController>();    
    }
    private void Update()
    {
        
        bool firing = PlayerAnim.GetBool("IsFiring");
        LaserAimedDeviceToggle(firing);
        RadarSystem();
         
        nearestTarget = GetClosestTarget(TargetsWithinAngle);
          

        
       if (firing && nearestTarget)
        {
            //some stuff here
            //firing and input di. magnitude = 0 do this 
            if (lockedTarget == null)
            {
                Debug.Log("loooking");
                LookManager(nearestTarget);
            }
            if (playerController.InputDir.magnitude == 0)
            {
                LookManager(nearestTarget);

            }
            /* if (lockedTarget == null)
             {
                 LookManager(nearestTarget);
                 lockedTarget = nearestTarget;
             }

             if (playerController.InputDir.magnitude == 0)
             {
                 LookManager(nearestTarget);
                 lockedTarget = nearestTarget;
             }*/

        }
       else
        {
            lockedTarget = null;

            //NearestTarget = null;

        }



    }
    void LookManager(Transform target)
    {


        Vector3 TargetDir = (target.position - transform.position).normalized;
        // transform.LookAt(NearestTarget.position);
        float targetRotAngle = (Mathf.Atan2(TargetDir.x, TargetDir.z)) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * targetRotAngle;
        lockedTarget = target;
    }

   

    void LaserAimedDeviceToggle(bool isFiring)
    {
        if (isFiring)
        {
            GunTip.gameObject.SetActive(true);
        }
        else
        {
            GunTip.gameObject.SetActive(false);

        }


        if ((Physics.Raycast(GunTip.transform.position, GunTip.transform.forward, out hit)))
        {
            if (hit.collider)
            {
                GunLine.SetPosition(0, GunTip.transform.position);
                GunLine.SetPosition(1, hit.point);
            }
        }
        else
        {
            GunLine.SetPosition(0, GunTip.transform.position);
            GunLine.SetPosition(1, GunTip.transform.forward * 500);

        }
        

        

    }
    void RadarSystem()
    {
        TargetsWithinAngle.Clear(); // this is important step
        Collider[] TargetsinRange = Physics.OverlapSphere(transform.position, PlayerVisionRadius, Targetmask);
        if (TargetsinRange.Length == 0)
        {
            TargetsWithinAngle.Clear();
            return;
        }
        else
        {

            for (int i = 0; i < TargetsinRange.Length; i++)
            {
                var tempTransform = TargetsinRange[i].transform;

                Vector3 TempDir = (tempTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, TempDir) < ViewingAngle / 2)
                {
                    //raycast then add
                    TargetsWithinAngle.Add(TargetsinRange[i].transform);

                }
            }
        }


    }
    Transform GetClosestTarget(List<Transform> targetsWithinAngle)
    { 
        Transform Nearest =null;
        var currentdistance =Mathf.Infinity; 
        if(targetsWithinAngle.Count>0)
        {
            for (int i = 0; i < targetsWithinAngle.Count; i++)
            {
                //  var NewDistance = (Targets[i].position - transform.position).sqrMagnitude;
                var NewDistance = Vector3.Distance(transform.position, targetsWithinAngle[i].position);

                if (NewDistance < currentdistance)
                {
                    currentdistance = NewDistance;
                    Nearest = targetsWithinAngle[i];

                }


            }
        }
   
        return Nearest;

    }


















    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PlayerVisionRadius);

        Vector3 CurrentRot = transform.eulerAngles;
        Vector3 directionR = new Vector3(Mathf.Sin(( (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad), 0,Mathf.Cos(((ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Vector3 directionL = new Vector3(Mathf.Sin(((ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad),0, Mathf.Cos(((ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, transform.position + directionR * PlayerVisionRadius);
        Gizmos.DrawLine(transform.position, transform.position + directionL * PlayerVisionRadius);

    }
}
