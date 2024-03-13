using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Surveilance : MonoBehaviour
{
    public Transform PathHolder;
    Vector3 PreviousPoint;
    Vector3 Startposition;
    public float RotationSpeed;
    public Light Spotlight;
    public float viewRadius;
    public float viewAngle;
    Transform player;
    public LayerMask viewMask;
    public float timeToSpotPlayer = .5f;
    [SerializeField]
    float PlayerVisibleTimer;
    [SerializeField]
    bool EnemySpotted;
   public  float RestTime;
    bool EmoteDirty;
    public Canvas Emote;
    [SerializeField]
    bool startroutine;
    [SerializeField]
    float emotetimer;
    [SerializeField]
    float emoteTimeToShow =1f;
    [SerializeField]
    Material CameraViewMat;
    public Renderer renderer;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    MaterialPropertyBlock block;
    static public event Action PlayerFound;

    // Start is called before the first frame update
    void Awake()
    {
         block = new MaterialPropertyBlock();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        startroutine = false;
        emotetimer = 0;
       // OGspotlightcolor = Spotlight.color;
        player = GameObject.FindWithTag("Player").transform;
       // viewAngle = Spotlight.spotAngle;
        Vector3[] Waypoints = new Vector3[PathHolder.childCount];
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Waypoints[i] = PathHolder.GetChild(i).position;
            Waypoints[i] = new Vector3(Waypoints[i].x, transform.position.y, Waypoints[i].z);


        }
        Emote.enabled = false;
        StartCoroutine(LookControl(Waypoints));
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSee())
        {

          
            PlayerVisibleTimer += Time.deltaTime;

           // Spotlight.color = Color.red;
        }
        else
        {
            PlayerVisibleTimer = 0f;
           // Spotlight.color = Color.yellow;
        }
        PlayerVisibleTimer = Mathf.Clamp(PlayerVisibleTimer, 0, timeToSpotPlayer);
        if (block != null)
        {
            block.SetColor("_Color", Color.Lerp(Color.yellow, Color.red, PlayerVisibleTimer / timeToSpotPlayer));
            renderer.SetPropertyBlock(block);
        }
        
       
        if (PlayerVisibleTimer >= timeToSpotPlayer)
        {
            //if (OnGuardHasSpotterPlayer != null)
            //{
            //    OnGuardHasSpotterPlayer();
            //}
          
            EnemySpotted = true;
            if (!EmoteDirty)
            {
                ShowSurprised();
            }
            if (PlayerFound != null)
            {
                PlayerFound();

            }
            Debug.Log("spotted");

        }
        else
        {
            EnemySpotted = false;
            EmoteDirty = false;

        }
    }
    private void LateUpdate()
    {
      //  DrawFieldOfView();
    }
    IEnumerator LookControl(Vector3[] Waypoints)
    {
        transform.LookAt(Waypoints[0]);
        int targetwaypointindex = 1;
        //look yield ret lookpathssssss
        Vector3 currentpath = Waypoints[targetwaypointindex];
        while (true)
        {
          
            yield return StartCoroutine(LookPath(currentpath));

            yield return new WaitForSeconds(RestTime);
            targetwaypointindex = (targetwaypointindex + 1) % Waypoints.Length;
            currentpath = Waypoints[targetwaypointindex];
        }

    }


    IEnumerator LookPath(Vector3 LookTarget)
    {
        

        
        Vector3 dirtolook = (LookTarget - transform.position).normalized;
        float targetangle = 90 - Mathf.Atan2(dirtolook.z, dirtolook.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetangle)) > 0.05f)
        {
            if (!EnemySpotted)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, RotationSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
            }
            yield return null;
        }
       




    }
    private void OnDrawGizmos()
    {
        Startposition = PathHolder.GetChild(0).position;
        PreviousPoint = Startposition;
        foreach (Transform Waypoints in PathHolder)
        {
            Gizmos.DrawSphere(Waypoints.position, .3f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Waypoints.position);
        }
       // Gizmos.DrawLine(PreviousPoint, Startposition);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,  viewRadius);

    }
    bool CanSee()
    {
        if (Vector3.Distance(transform.position, player.position) < viewRadius)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            dirToPlayer.y = 0;
            float angleBetweenGuardandPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardandPlayer < viewAngle /2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask)) // if not wall
                {
                    //EnemySpotted = true;
                    if (!player.GetComponent<PlayerController>().InBox)
                    {
                        return true;
                    }
                    else if(player.GetComponent<PlayerController>().InBox && player.GetComponent<PlayerController>().InputDir.magnitude >0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }


        }
       // EnemySpotted = false;
        return false;
    }

    void  ShowSurprised()
    {
        Debug.Log("here");
        
        Emote.enabled = true;
        emotetimer += Time.deltaTime;
        Emote.GetComponent<CanvasGroup>().alpha = emoteTimeToShow/emotetimer;
        if (emotetimer > emoteTimeToShow )
        {
            emotetimer = 0;
            Emote.enabled = false;
            Emote.GetComponent<CanvasGroup>().alpha = 0;

            EmoteDirty = true;

            //  return;

        }

       
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius,viewMask)) //playermask
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }



}
