using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;
    private float acceleration = 5f, maxVelocity = 1f;
    private Vector2 velocity = Vector2.zero;
    
    private Vector3 cameraRotation;


    private void Update()
    {

        //cameraRotation = Vector3.ProjectOnPlane(Camera.main.transform.rotation.eulerAngles.normalized, Vector3.up);
        
        Vector3 povorot = new Vector3(InputHandler.instance.InputVector.x, 0, InputHandler.instance.InputVector.y);

        Quaternion cameraRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        povorot =  cameraRotation * povorot;
        Vector2 dir = new Vector2(povorot.x, povorot.z);
        velocity = Vector2.MoveTowards(velocity, dir * maxVelocity, acceleration * Time.deltaTime);



        playerAnimator.SetFloat("Vertical", velocity.y);
        playerAnimator.SetFloat("Horisontal", velocity.x);
        playerAnimator.SetBool("Crouch", InputHandler.instance.isCtrlPressed);
    }
}
