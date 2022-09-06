using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;
    private InputHandler input;

    private void Awake()
    {
        input = GetComponent<InputHandler>();
    }
    private void Update()
    {
        playerAnimator.SetFloat("Vertical", input.InputVector.y);
        playerAnimator.SetFloat("Vertical", input.InputVector.x);
    }
}
