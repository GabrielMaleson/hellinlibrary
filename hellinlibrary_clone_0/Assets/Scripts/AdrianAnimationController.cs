using UnityEngine;

public class AdrianAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private HumanController humanController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        humanController = GetComponent<HumanController>();
    }

    private void Update()
    {
        if (humanController == null || !humanController.photonView.IsMine) return;

        // Get movement input from HumanController
        Vector3 movement = humanController.Movement;

        // Normalize the movement vector to ensure consistent speed
        movement.Normalize();

        // Set BlendTree parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.z);

        // Handle sprite flipping based on horizontal direction
        if (movement.x > 0)
        {
            // Facing right (no flip)
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            // Facing left (flip)
            spriteRenderer.flipX = true;
        }
    }
}