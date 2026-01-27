using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The player transform to follow. If not set, will use parent on start.")]
    public Transform target;

    [Header("X Axis Boundaries")]
    public float minX = -50f;
    public float maxX = 50f;

    [Header("Z Axis Boundaries")]
    public float minZ = -50f;
    public float maxZ = 50f;

    [Header("Offset")]
    [Tooltip("The offset from the target position (automatically calculated from initial parent offset if left at zero)")]
    public Vector3 offset;

    private void Start()
    {
        // If no target is set, use the parent as the target
        if (target == null && transform.parent != null)
        {
            target = transform.parent;
        }

        // Calculate offset from parent if offset is zero
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }

        // Unparent the camera so we can control its position independently
        transform.SetParent(null);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position based on target + offset
        Vector3 desiredPosition = target.position + offset;

        // Clamp the X and Z coordinates
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedZ = Mathf.Clamp(desiredPosition.z, minZ, maxZ);

        // Apply the clamped position (Y stays the same as desired)
        transform.position = new Vector3(clampedX, desiredPosition.y, clampedZ);
    }

    // Visualize the boundaries in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Draw boundary lines
        float y = transform.position.y;
        float height = 10f; // Visual height for the boundary lines

        // Draw the rectangular boundary
        Vector3 bottomLeft = new Vector3(minX, y, minZ);
        Vector3 bottomRight = new Vector3(maxX, y, minZ);
        Vector3 topLeft = new Vector3(minX, y, maxZ);
        Vector3 topRight = new Vector3(maxX, y, maxZ);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw vertical lines at corners
        Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.up * height);
        Gizmos.DrawLine(bottomRight, bottomRight + Vector3.up * height);
        Gizmos.DrawLine(topLeft, topLeft + Vector3.up * height);
        Gizmos.DrawLine(topRight, topRight + Vector3.up * height);
    }
}
