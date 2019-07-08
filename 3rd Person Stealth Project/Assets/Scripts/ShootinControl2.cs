using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShootinControl2 : MonoBehaviour
{
    public Transform GunTip;
    bool ShootingStance;
    public Animator PlayerAnim;
    RaycastHit hit;
    public float PlayerVisionRadius = 5f;
    public LayerMask Targetmask;
    [SerializeField]
    Transform NearestTarget;
    //Transform[] TargetsInAngle;
    public float ViewingAngle;
    [SerializeField]
    List<Transform> TargetsWithinAngle; 
    public LineRenderer GunLine;
    void Start()
    {
        TargetsWithinAngle = new List<Transform>();
        GunLine = GunTip.GetComponent<LineRenderer>();

    }
    private void Update()
    {
        
        bool Firing = PlayerAnim.GetBool("IsFiring");
        LaserSystem(Firing);
        RadarSystem();
      
        if (TargetsWithinAngle.Count != 0)

        {
            NearestTarget = GetClosestTarget(TargetsWithinAngle);
           // Debug.Log(NearestTarget.ToString());
           // Debug.Log(GetClosestTarget(TargetsWithinAngle).ToString());

        }
       if (Firing && NearestTarget)
        {
            LookManager(NearestTarget);
        }
       else
        {
            //NearestTarget = null;

        }



    }
    void LookManager(Transform target)
    {


        Vector3 TargetDir = (target.position - transform.position).normalized;
        // transform.LookAt(NearestTarget.position);
        float targetRotAngle = (Mathf.Atan2(TargetDir.x, TargetDir.z)) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * targetRotAngle;
    }

   

    void LaserSystem(bool isFiring)
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
            GunLine.SetPosition(1, GunTip.transform.position * 500);

        }
        

        

    }
    void RadarSystem()
    {
        TargetsWithinAngle.Clear(); // this is important step
        NearestTarget = null;
        Collider[] TargetsinRange = Physics.OverlapSphere(transform.position, PlayerVisionRadius, Targetmask);
        if (TargetsinRange.Length == 0)
        {
            TargetsWithinAngle.Clear();
            return;
        }

        for (int i = 0; i < TargetsinRange.Length; i++)
        {
            var tempTransform = TargetsinRange[i].transform;

            Vector3 TempDir = (tempTransform.position-transform.position  ).normalized;

            if (Vector3.Angle(transform.forward, TempDir) < ViewingAngle/2)
            {
                //raycast then add
                TargetsWithinAngle.Add(TargetsinRange[i].transform);

            }
        }


    }
    Transform GetClosestTarget(List<Transform> Targets)
    { 
        Transform Nearest =null;
        var currentdistance =Mathf.Infinity; 
        for(int i =0; i < Targets.Count; i++)
        {
            //  var NewDistance = (Targets[i].position - transform.position).sqrMagnitude;
            var NewDistance = Vector3.Distance(transform.position, Targets[i].position);

            if (NewDistance < currentdistance)
            {
                currentdistance = NewDistance;
                Nearest = Targets[i];
                
            }


        }
        return Nearest;

    }


















    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PlayerVisionRadius);
        Gizmos.color = Color.red;
        Vector3 CurrentRot = transform.eulerAngles;
        Vector3 directionR = new Vector3(Mathf.Cos((90 - (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad), 0, Mathf.Sin((90 - (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Vector3 directionL = new Vector3(Mathf.Cos((90 - (ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad), 0, Mathf.Sin((90 - (ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, transform.position + directionR * PlayerVisionRadius);
        Gizmos.DrawLine(transform.position, transform.position + directionL * PlayerVisionRadius);

    }
}
