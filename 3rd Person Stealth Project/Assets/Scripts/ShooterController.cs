using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    List<Transform> TargetsInRange; 
    public LineRenderer GunLine;
    [SerializeField] GameObject hitVFXPrefab;
    void Start()
    {
        TargetsWithinAngle = new List<Transform>();
        GunLine = GunTip.GetComponent<LineRenderer>();
        playerController = GetComponent<PlayerController>();
       
    }
    private void OnEnable()
    {
        PlayerInput.onReleaseClick += PlayerInput_onReleaseClick;
    }

    private void OnDisable()
    {
        PlayerInput.onReleaseClick -= PlayerInput_onReleaseClick;

    }


    private void PlayerInput_onReleaseClick()
    {
        Debug.Log(playerController.GetIsInFireArmStance());

      if( ! playerController.InBox)
            Shoot();
        
    }

    private void Update()
    {
        
        bool firingPos = PlayerAnim.GetBool("IsFiring");
        LaserAimedDeviceToggle(firingPos);
        
        if(firingPos) RadarSystem();
         
        nearestTarget = GetClosestTarget(TargetsWithinAngle);
          

        
       if (firingPos && nearestTarget)
        {
            //some stuff here
            //firing and input di. magnitude = 0 do this 
            if (lockedTarget == null)
            {
                Debug.Log("loooking");
                LookManager(nearestTarget);
            }
            if (playerController.inputDir.magnitude == 0)
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
        Debug.Log("look adjusted");
        var pos = target.position;
      //  pos.y = transform.position.y;
        Vector3 TargetDir = (pos - transform.position).normalized;
        //  transform.LookAt(TargetDir);
        float targetRotAngle = (Mathf.Atan2(TargetDir.x, TargetDir.z)) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * targetRotAngle;
        lockedTarget = target;
    }

    void Shoot()
    {
        Debug.Log("PEW FROM SHOOT CONTROLLER");

        if (lockedTarget != null)
        {

            if (Physics.Raycast(GunTip.transform.position , lockedTarget.position - GunTip.transform.position, out RaycastHit hit))
            {

                Instantiate(hitVFXPrefab, hit.point + (GunTip.transform.position - hit.point).normalized *.1f , Quaternion.LookRotation((GunTip.transform.position - hit.point).normalized));


              if (  hit.transform.TryGetComponent<Itargetable>(out Itargetable target))
                target.TakeHit();
            }
        }
        else
        {
            if (Physics.Raycast(GunTip.transform.position, GunTip.transform.forward, out RaycastHit hit))
            {
                Instantiate(hitVFXPrefab, hit.point + (GunTip.transform.position - hit.point).normalized * .1f, Quaternion.LookRotation((GunTip.transform.position - hit.point).normalized));
                if ( hit.transform.TryGetComponent<Itargetable>(out Itargetable target))
                target.TakeHit();

            }
        }
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

    /*    if (lockedTarget)
        {

            if ((Physics.Raycast(GunTip.transform.position, (lockedTarget.transform.position -  GunTip.transform.position).normalized, out hit)))
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

        }*/


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

        float height = 1.5f;
        Vector3 adjustedCentralPos = transform.position + Vector3.up * height;
        TargetsWithinAngle.Clear(); // this is important step
        Collider[] TargetsinRange = Physics.OverlapSphere(adjustedCentralPos, PlayerVisionRadius, Targetmask);
     //   Debug.Log(TargetsinRange.Count());
        if (TargetsinRange.Length == 0)
        {
            TargetsWithinAngle.Clear();
            return;
        }
        else
        {

            for (int i = 0; i < TargetsinRange.Length; i++)
            {
                var tempTransform = TargetsinRange[i].transform ;
                var tempPos = new Vector3(TargetsinRange[i].transform.position.x, adjustedCentralPos.y, TargetsinRange[i].transform.position.z);

               Debug.Log("in range is " +tempTransform.name);
                Vector3 TempDir = (tempPos - adjustedCentralPos).normalized;

                if (Vector3.Angle(transform.forward, TempDir) < ViewingAngle / 2)
                {
                    Debug.Log("inside angle is " + tempTransform.name);

                    // TargetsWithinAngle.Add(TargetsinRange[i].transform);

                    //raycast then add
                    
                    Debug.DrawRay(adjustedCentralPos, (tempTransform.position - adjustedCentralPos));

                    if (Physics.Raycast(adjustedCentralPos, (tempTransform.position - adjustedCentralPos).normalized, out RaycastHit hit,PlayerVisionRadius))
                    {

                        if (hit.collider.CompareTag("Enemy"))
                        {
                            TargetsWithinAngle.Add(TargetsinRange[i].transform);

                        }
                        else
                        {
                            Debug.Log(hit.transform.name);
                        }

                      
                    }
                    else
                    {
                        Debug.Log("CLEAR SHOT");

                    }

                }
            }
        }


    }
    Transform GetClosestTarget(List<Transform> targetsWithinAngle)
    { 
        Transform Nearest =null;
        var currentdistance =Mathf.Infinity;
        float shootheigthLimiter = 3f;
        if(targetsWithinAngle.Count>0)
        {
            for (int i = 0; i < targetsWithinAngle.Count; i++)
            {
                if (Mathf.Abs(transform.position.y - targetsWithinAngle[i].position.y) > shootheigthLimiter) Debug.Log("heightPass failed" );
                    if (Mathf.Abs(transform.position.y - targetsWithinAngle[i].position.y) > shootheigthLimiter) continue;
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
        Gizmos.DrawWireSphere(GunTip.transform.position, PlayerVisionRadius);

        Vector3 CurrentRot = transform.eulerAngles;
        Vector3 directionR = new Vector3(Mathf.Sin(( (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad), 0,Mathf.Cos(((ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Vector3 directionL = new Vector3(Mathf.Sin(((ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad),0, Mathf.Cos(((ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, transform.position + directionR * PlayerVisionRadius);
        Gizmos.DrawLine(transform.position, transform.position + directionL * PlayerVisionRadius);

    }
}
