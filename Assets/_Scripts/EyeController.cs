using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{

    //private Transform player;
    private Transform playerTop;
    private Transform lookAt;

    private void Start()
    {
        //player = FindObjectOfType<PlayerMove>().transform;
        playerTop = FindObjectOfType<LookPointTag>().transform;

    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, playerTop.position - transform.position, out hit  /*, Vector3.Distance(transform.position, playerTop.position)*/))
        { 
            if (!hit.transform.CompareTag("Bush"))
            {
                

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerTop.position - transform.position, Vector3.up), Time.deltaTime);
                
            }
            else
                Debug.Log("i`m pretending that i don`t see you, but i see you");
        }

    }
}
