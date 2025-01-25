using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player; // The player to follow
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Offset to keep the camera behind the player

    private Vector3 _velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (player is null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // Desired position of the camera based on the player's position and offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}