using UnityEngine;

public class cameraScript : MonoBehaviour
{
    [Tooltip("Drag your Player GameObject here")]
    public Transform target;
    [Tooltip("Offset from the player's position")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    [Tooltip("Smoothness of camera movement")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }
}
