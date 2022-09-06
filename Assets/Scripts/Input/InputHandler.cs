using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-239)]
public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    public Vector2 InputVector { get; private set; }
    public Vector2 MousePosition { get; private set; }



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
    }

    private void Update()
    {
        var h = Input.GetAxis("Horisontal");
        var v = Input.GetAxis("Vertical");
        InputVector = new Vector2(h, v);

        MousePosition = Input.mousePosition;
    }
}
