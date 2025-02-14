using UnityEngine;
using Photon.Pun; // Import Photon namespace
using Photon.Realtime;

public class AdrianAnimationController : MonoBehaviour, IPunObservable
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private HumanController humanController;
    private Transform textTransform; // Reference to the Text TMP GameObject

    private float syncedHorizontal;
    private float syncedVertical;
    private bool syncedFlipX; // Variable to sync sprite flipping

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        humanController = GetComponent<HumanController>();

        // Assuming the TextMeshPro object is a child named "Text"
        textTransform = transform.Find("Text");
    }

    private void Update()
    {
        if (humanController == null || !humanController.photonView.IsMine)
        {
            // Use the synchronized values for animations and flipX on remote clients
            animator.SetFloat("Horizontal", syncedHorizontal);
            animator.SetFloat("Vertical", syncedVertical);
            spriteRenderer.flipX = syncedFlipX;
            return;
        }

        // Get movement input from HumanController
        Vector3 movement = humanController.Movement;

        // Normalize the movement vector to ensure consistent speed
        movement.Normalize();

        // Set BlendTree parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.z);

        // Handle sprite flipping based on horizontal direction
        spriteRenderer.flipX = movement.x < 0;

        // Ensure the text does not flip
        if (textTransform != null)
        {
            Vector3 textScale = textTransform.localScale;
            textScale.x = spriteRenderer.flipX ? -Mathf.Abs(textScale.x) : Mathf.Abs(textScale.x);
            textTransform.localScale = textScale;
        }
    }

    // Photon synchronization for animation parameters and flipX
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // Local player sending data
        {
            stream.SendNext(animator.GetFloat("Horizontal"));
            stream.SendNext(animator.GetFloat("Vertical"));
            stream.SendNext(spriteRenderer.flipX); // Sync flipX value
        }
        else // Remote player receiving data
        {
            syncedHorizontal = (float)stream.ReceiveNext();
            syncedVertical = (float)stream.ReceiveNext();
            syncedFlipX = (bool)stream.ReceiveNext(); // Receive flipX value
        }
    }
}
