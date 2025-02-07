using UnityEngine;

public class DevilAnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;

    // BlendTree parameters
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    private void Awake()
    {
        // Get the Animator component
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component is missing on the Player GameObject.");
            return;
        }

        // Get the PlayerController component
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController component is missing on the Player GameObject.");
            return;
        }
    }

    private void Update()
    {
        if (!_playerController.photonView.IsMine) return;

        // Get the movement vector from the PlayerController
        Vector3 movement = _playerController.Movement;

        // Normalize the movement vector to ensure consistent speed in all directions
        movement.Normalize();

        // Set the BlendTree parameters based on movement direction
        _animator.SetFloat(Horizontal, movement.x);
        _animator.SetFloat(Vertical, movement.z);

        // Handle flipping the sprite for left/right movement
        if (movement.x != 0)
        {
            // Flip the sprite by inverting the X scale
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(movement.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}