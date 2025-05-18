using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    public float minX, maxX, minY, maxY;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
