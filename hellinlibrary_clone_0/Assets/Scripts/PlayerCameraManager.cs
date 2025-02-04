using UnityEngine;
using Photon.Pun;

public class PlayerCameraManager : MonoBehaviourPunCallbacks
{
    public GameObject playerCameraPrefab; // Prefab for the player camera
    public float cameraHeight = 50f; // Height of the camera above the player
    public float cameraAngle = 135f; // Angle of the camera (top-down view)

    private GameObject playerCameraInstance;

    void Start()
    {
        // Check if the current instance is controlled by the local player
        if (photonView.IsMine)
        {
            CreatePlayerCamera();
        }
    }

    void CreatePlayerCamera()
    {
        if (playerCameraPrefab != null)
        {
            // Instantiate the camera for the local player
            playerCameraInstance = Instantiate(playerCameraPrefab, transform.position, Quaternion.identity);

            // Position the camera above the player at a fixed angle
            Vector3 cameraPosition = transform.position + Vector3.up * cameraHeight;
            playerCameraInstance.transform.position = cameraPosition;

            // Rotate the camera to look down at the player
            playerCameraInstance.transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);

            // Set the camera's parent to the player so it follows the player
            playerCameraInstance.transform.SetParent(transform);
        }
        else
        {
            Debug.LogError("PlayerCameraPrefab is not assigned in the PlayerCameraManager script.");
        }
    }

    void OnDestroy()
    {
        // Destroy the camera instance when the player is destroyed
        if (playerCameraInstance != null)
        {
            Destroy(playerCameraInstance);
        }
    }
}