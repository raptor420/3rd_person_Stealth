using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;
public class FogLight : BaseClassSpotter, Itargetable
{

    public Collider detectionCollider;
    public Transform lightObject;
    [SerializeField] float timeToDetectPlayer = 1;
    [SerializeField] float timer;
    [SerializeField] Light light;
    [SerializeField] Color lightColor;
    [SerializeField] Color redColor;
   [SerializeField] bool playerSighted;
    [SerializeField] GameObject glassBreakVFXPrefab;
    bool destroyed = false;

    Vector3 startingAngle;
    Vector3 targetAngle;
    float currAngleX;
    [SerializeField]  float rotateSpeed = 5;
    float moveTimer = 3f;
    Sequence sequence;
    private void Start()
    {
        startingAngle = lightObject.rotation.eulerAngles;
        currAngleX = startingAngle.x;
        targetAngle = new Vector3(lightObject.rotation.eulerAngles.x + 20, lightObject.rotation.eulerAngles.y, lightObject.rotation.eulerAngles.z);

        StartMoveLightRotate();
    }

    private void StartMoveLightRotate()
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(targetAngle, 3));
        sequence.AppendInterval(1);
        sequence.Append(transform.DORotate(startingAngle, 3));
        sequence.AppendInterval(1);
        sequence.SetLoops(-1);
        sequence.Play();
    }

    void    RotateLight()
    {

        if (currAngleX < targetAngle.x)
        {
            currAngleX+= Time.deltaTime* rotateSpeed;
            transform.eulerAngles = new Vector3(currAngleX, 0, 0);
            // Vector3.RotateTowards()
           // mathf.MoveTowardsAngle();
        }
    }

    void Rotatelight2()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(targetAngle.x,0,0),Time.deltaTime * rotateSpeed);
    }

    public void TakeHit()
    {
        if(destroyed) return;
        detectionCollider.enabled = false;
        light.enabled=(false);
        destroyed=true;
        transform.DOKill();
        Instantiate(glassBreakVFXPrefab,lightObject.transform.position,glassBreakVFXPrefab.transform.rotation);
    }

    public void Update()
    {
        if (playerSighted)
        {
            timer += Time.deltaTime;
            sequence.Pause();
        }

        else
        {
            timer = 0;
            sequence.Play();

        }

        light.color = Color.Lerp(lightColor,redColor, timer / timeToDetectPlayer);


        if( timer >= timeToDetectPlayer)
        {
            PlayerSpotted();
            Debug.Log("HasBeenSighted");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("W3TTTTGHHH");
            playerSighted = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSighted = false;
        }
    }

    public override void PlayerSpotted()
    {
        OnPlayerSpotted();
    }
}
