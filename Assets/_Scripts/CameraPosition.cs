using UnityEngine;

public class CameraPosition : MonoBehaviour
{

    private Vector3 offset;
    [SerializeField]
    private Transform player;
    private Vector3 previousPosition;

    void Start()
    {
        offset = transform.position;
        previousPosition = transform.position;
}

    private void Update()
    {
        transform.position = Vector3.Lerp(previousPosition, player.position + offset, Time.deltaTime);
        previousPosition = transform.position;
        //transform.position = player.position + offset;
    }
}
