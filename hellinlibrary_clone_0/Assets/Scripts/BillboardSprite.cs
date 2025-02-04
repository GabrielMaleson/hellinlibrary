using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Camera playerCamera;

    void Start()
    {
        // Find the player's camera
        playerCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Make the sprite face the camera
            transform.LookAt(playerCamera.transform);

            // Lock the sprite's rotation to only the Y-axis (optional, depending on your game)
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}