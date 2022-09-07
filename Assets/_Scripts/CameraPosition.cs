using UnityEngine;

public class CameraPosition : MonoBehaviour
{

    private Vector3 offset;
    
    [SerializeField]
    private Transform player;

    void Start()
    {
        offset = transform.position - player.position;    
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + offset, 0.239f);
       
        if (InputHandler.instance.isMouseWheelPressed)
        {
            transform.position = player.position + offset;
            transform.RotateAround(player.position, Vector3.up, InputHandler.instance.MouseDelta.x);
            offset = transform.position - player.position;
        }

    }
}
