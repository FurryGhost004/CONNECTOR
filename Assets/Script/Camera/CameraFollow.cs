using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí muốn đến
        Vector3 desiredPosition = target.position + offset;

        // Camera luôn ở z = -10
        desiredPosition.z = -10;

        // Lerp để mượt
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Gán lại vị trí camera
        transform.position = smoothedPosition;
    }
}
