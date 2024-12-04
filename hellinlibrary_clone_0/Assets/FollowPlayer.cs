using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform Player;   // The player transform to follow
    public Vector3 offset;     // The offset distance between the camera and the player
    public float smoothSpeed = 0.125f; // Camera smoothness factor

    void LateUpdate()
    {
        Vector3 desiredPosition = Player.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        transform.LookAt(Player);
    }
}