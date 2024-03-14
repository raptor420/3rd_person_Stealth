using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FogLight : MonoBehaviour, Itargetable
{

    public Collider detectionCollider;
    public Transform lightObject;
    [SerializeField] float timeToDetectPlayer = 1;
    [SerializeField] float timer;
    [SerializeField] Light light;
    [SerializeField] Color lightColor;
    [SerializeField] Color redColor;
   [SerializeField] bool playerSighted;

    public void TakeHit()
    {
        detectionCollider.enabled = false;
        lightObject.gameObject.SetActive(false);  
    }

    public void Update()
    {
        if (playerSighted)
        {
            timer += Time.deltaTime;

        }

        else
        {
            timer = 0;

        }

        light.color = Color.Lerp(lightColor,redColor, timer / timeToDetectPlayer);


        if( timer >= timeToDetectPlayer)
        {
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
}
