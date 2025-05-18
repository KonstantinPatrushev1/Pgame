using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform character;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    
    public int pixelsPerUnit = 16;

    void FixedUpdate()
    {
        Vector3 desiredPosition = character.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Принудительно фиксируем Z
        smoothedPosition.z = -10f;

        // Привязка к пиксельной сетке
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = smoothedPosition;
    }
}