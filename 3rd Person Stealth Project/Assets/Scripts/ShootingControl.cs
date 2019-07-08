using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform GunTip;
    bool ShootingStance;
   public  Animator PlayerAnim;
    RaycastHit hit;
    public float PlayerVisionRadius=5f;
   public LayerMask Targetmask;
    [SerializeField]
    Transform NearestTarget;
    Transform[] TargetsInAngle;
    public float ViewingAngle ;
    [SerializeField]
    List<Transform> TargetsWithinAngle;
    public LineRenderer GunLine;
    void Start()
    {
        TargetsWithinAngle = new List<Transform>(); 
        GunLine = GunTip.GetComponent<LineRenderer>();
      
    }

    // Update is called once per frame
    void Update()
    {
      


        if (PlayerAnim.GetBool("IsFiring")) //|| PlayerAnim.GetBool("Fire") can be added to fire laser for a bit
        {
            TargetsWithinAngle.Clear();
            GunTip.gameObject.SetActive(true);

            if (TargetSpotted())

            {
                if (!TargetsWithinAngle[0] == null)
                {
                    Transform nearest = TargetsWithinAngle[0];
                    float currentdistance = (TargetsWithinAngle[0].position - transform.position).sqrMagnitude;
                    for (int i = 1; i < TargetsWithinAngle.Count; i++)
                    {
                        float Newdistance = (TargetsWithinAngle[i].position - transform.position).sqrMagnitude;
                        if (Newdistance < currentdistance)
                        {
                            NearestTarget = TargetsWithinAngle[i];
                            Debug.DrawLine(transform.position, NearestTarget.position);
                        }
                        //maybe else
                        else
                        {
                            NearestTarget = nearest;
                        }
                    }
                }

                if (NearestTarget != null)
                {
                    Vector3 TargetDir = (NearestTarget.position - transform.position).normalized;
                    // transform.LookAt(NearestTarget.position);
                    float targetRotAngle = (Mathf.Atan2(TargetDir.x, TargetDir.z)) * Mathf.Rad2Deg;
                    transform.eulerAngles = Vector3.up * targetRotAngle;
                }

            }
          


            if (Physics.Raycast(GunTip.transform.position, GunTip.transform.forward, out hit))
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
                GunLine.SetPosition(1, GunTip.transform.forward * 500f);

            }
        }
        else
        {
            GunTip.gameObject.SetActive(false);
            NearestTarget = null;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PlayerVisionRadius);



    }

    bool TargetSpotted()
    {
        //TargetsWithinAngle.Clear();
       Collider[] colliders = Physics.OverlapSphere(transform.position, PlayerVisionRadius,Targetmask);
      
       
       
        if (colliders.Length == 0)
        {
            NearestTarget = null;
            return false;
        }


        //  NearestTarget = colliders[0].transform;
        //  float currentDistance = (NearestTarget.position - transform.position).sqrMagnitude;
        foreach (Collider collider in colliders)
        {
            Vector3 DirtoTarget = (collider.transform.position - transform.position).normalized;
            float anglebetweenTarget = Vector3.Angle(transform.forward, DirtoTarget);
            if (anglebetweenTarget < ViewingAngle/2 )
            {

                if (Physics.Raycast(transform.position, collider.transform.position, Targetmask))
                {
                   // NearestTarget = collider.transform;
                    TargetsWithinAngle.Add(collider.transform);
                  //  return true;

                }

                // return false;
            }
            

            //if (TargetsWithinAngle !=null)
            //{
            //    NearestTarget = TargetsWithinAngle[0];
            //    float currentDistance = (NearestTarget.position - transform.position).sqrMagnitude;
            //    foreach (Transform t in TargetsWithinAngle) // checking stored transforms in angle
            //    {
            //        float newDistance = (t.position - transform.position).sqrMagnitude;
            //        if (newDistance < currentDistance/2f)
            //        {

            //            NearestTarget = t;
            //            return true;
            //        }
            //    }
            //}
            //else
            //{
            //    NearestTarget = null;
            //    return false;
            //}

        }

        
            //if (collider != colliders[0])
            //{

            //    float NewDistance = (collider.transform.position - transform.position).sqrMagnitude;

            //    if(NewDistance < currentDistance)
            //    {

            //        NearestTarget = collider.transform;

            //        Vector3 DirToNearestTarget = (NearestTarget.position - transform.position);
            //        float angleBetweenTargetandPlayer = Vector3.Angle(transform.forward, DirToNearestTarget);
            //        if (angleBetweenTargetandPlayer < ViewingAngle / 2)
            //        {

            //            return true;


                     
            //        }
                   

            //    }



            //}


            
        return true;



    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Vector3 CurrentRot = transform.localEulerAngles;
        //Vector3 directionR = new Vector3(Mathf.Sin( ((90 - ViewingAngle / 2) +CurrentRot.y ) * Mathf.Deg2Rad), 0, Mathf.Cos(((90 - ViewingAngle / 2) + CurrentRot.y) * Mathf.Deg2Rad)).normalized;
        //Vector3 directionL = new Vector3(Mathf.Cos(((90 - ViewingAngle /- 2) +CurrentRot.y) * Mathf.Deg2Rad), 0, Mathf.Sin(((90 - ViewingAngle / -2) + CurrentRot.y) * Mathf.Deg2Rad)).normalized;
        //// Vector3 direction = new Vector3(Mathf.Cos((90 - ViewingAngle) * Mathf.Deg2Rad), 0, Mathf.Sin((90 - ViewingAngle) * Mathf.Deg2Rad));
        ////Vector3 direction = new Vector3(Mathf.Cos(( ViewingAngle) * Mathf.Deg2Rad), 0, Mathf.Sin(( ViewingAngle) * Mathf.Deg2Rad));
        //Gizmos.DrawRay(transform.position ,  directionR * PlayerVisionRadius);
        //Gizmos.DrawRay(transform.position  , directionL * PlayerVisionRadius);

        //Debug.Log("R"+(90 - ViewingAngle / 2) + CurrentRot.y);
        //Debug.Log("L" +(90 - ViewingAngle / -2) + CurrentRot.y);
        Gizmos.color = Color.red;
        Vector3 CurrentRot = transform.eulerAngles;
        Vector3 directionR = new Vector3(Mathf.Cos((90 - (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad), 0, Mathf.Sin((90 - (ViewingAngle / 2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Vector3 directionL = new Vector3(Mathf.Cos((90 - (ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad), 0, Mathf.Sin((90 - (ViewingAngle / -2 + CurrentRot.y)) * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, transform.position + directionR * PlayerVisionRadius);
        Gizmos.DrawLine(transform.position, transform.position + directionL * PlayerVisionRadius);
        

    }
    

}



    //void  SetShootingStance(bool stance)
    // {

    //     ShootingStance = stance;


    // }


