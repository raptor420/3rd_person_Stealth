using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Transform PathHolder;
    Vector3 PreviousPoint;
    Vector3 Startposition;
    public float speed = 5;
    public float waitTime = .5f;
    public float RotationSpeed;
    public Light Spotlight;
    public float viewDistance;
        float viewAngle;
    Transform player;
    public LayerMask viewMask;
    Color OGspotlightcolor;
    public float timeToSpotPlayer =.5f;
    float PlayerVisibleTimer;
    public static event Action OnGuardHasSpotterPlayer;
    [SerializeField]
    bool EnemySpotted;
    // Start is called before the first frame update
    void Awake()
    {
        OGspotlightcolor = Spotlight.color;
        player = GameObject.FindWithTag("Player").transform;
        viewAngle = Spotlight.spotAngle;
        Vector3[] Waypoints = new Vector3 [PathHolder.childCount];
        for (int i =0; i < Waypoints.Length; i++)
        {
            Waypoints[i] = PathHolder.GetChild(i).position;
            Waypoints[i] = new Vector3(Waypoints[i].x, transform.position.y, Waypoints[i].z);
        

        }
        StartCoroutine(FollowPath (Waypoints));
        
    }
    bool CanSee()
    {
        if(Vector3.Distance(transform.position,player.position)< viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardandPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardandPlayer < viewAngle/2)
            {
                if (!Physics.Linecast(transform.position,player.position,viewMask)) {
                    EnemySpotted = true;
                    return true;
                    
                }
            }


        }
        EnemySpotted = false;
        return false;
    }


    

    IEnumerator FollowPath(Vector3[] Waypoints)
    {
        transform.position = Waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = Waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint,speed *Time.deltaTime);
            if(transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % Waypoints.Length;
                targetWaypoint = Waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
               yield return StartCoroutine (Turntoface (targetWaypoint));

            }




            yield return null;
            }

    }

    void lookAtTarget(Vector3 target)
    {

        //transform.LookAt(target);
       // transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(target- transform.position),RotationSpeed * Time.deltaTime );

    }
    IEnumerator Turntoface (Vector3 LookTarget)
    {
        Vector3 dirtolook = (LookTarget - transform.position).normalized;
        float targetangle =90- Mathf.Atan2(dirtolook.z, dirtolook.x) * Mathf.Rad2Deg;
        while (Mathf.Abs( Mathf.DeltaAngle(transform.eulerAngles.y,targetangle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, RotationSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSee())
        {
            Debug.Log("spotted");
            PlayerVisibleTimer += Time.deltaTime;
           // Spotlight.color = Color.red;

        }
        else
        {
            PlayerVisibleTimer -= Time.deltaTime;
            //Spotlight.color = OGspotlightcolor;
        }
        PlayerVisibleTimer = Mathf.Clamp(PlayerVisibleTimer, 0, timeToSpotPlayer);
        Spotlight.color = Color.Lerp(OGspotlightcolor, Color.red,PlayerVisibleTimer/timeToSpotPlayer);
        if(PlayerVisibleTimer >= timeToSpotPlayer)
        {
            if(OnGuardHasSpotterPlayer != null)
            {
                OnGuardHasSpotterPlayer();
            }

        }
    }
    private void OnDrawGizmos()
    {
        Startposition = PathHolder.GetChild(0).position;
        PreviousPoint = Startposition;
        foreach (Transform Waypoints in PathHolder)
        {
            Gizmos.DrawSphere(Waypoints.position, .3f);
            Gizmos.DrawLine(PreviousPoint, Waypoints.position);
            PreviousPoint = Waypoints.position;
        }
        Gizmos.DrawLine(PreviousPoint, Startposition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);

    }


}
