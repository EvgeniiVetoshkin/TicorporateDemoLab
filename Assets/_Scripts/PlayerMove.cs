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

        cameraRotation = Vector3.ProjectOnPlane(Camera.main.transform.rotation.eulerAngles.normalized, Vector3.up);
        

        velocity = Vector2.MoveTowards(velocity, InputHandler.instance.InputVector * maxVelocity, acceleration * Time.deltaTime);
            
        playerAnimator.SetFloat("Vertical", velocity.y);
        playerAnimator.SetFloat("Horisontal", velocity.x);
        playerAnimator.SetBool("Crouch", InputHandler.instance.isCtrlPressed);
    }
}
