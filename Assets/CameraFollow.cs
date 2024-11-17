using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform character; 
    public Vector3 offset;   
    public float smoothSpeed = 0.125f; 

    void FixedUpdate()
    {

        Vector3 desiredPosition = character.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

    }
}