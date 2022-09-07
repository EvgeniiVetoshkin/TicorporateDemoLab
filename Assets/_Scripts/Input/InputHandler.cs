using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-239)]
public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    public Vector2 InputVector { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool isCtrlPressed { get; private set; }


    private InputActions inputActions;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        inputActions = new InputActions();
        inputActions.CharacterControl.Enable();
    }

    private void Update()
    {
        
        InputVector = inputActions.CharacterControl.Move.ReadValue<Vector2>();
        MousePosition = Input.mousePosition;
        isCtrlPressed = inputActions.CharacterControl.Crouch.IsPressed();
    }
}
