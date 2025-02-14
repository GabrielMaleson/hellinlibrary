using System;
using Photon.Pun;
using UnityEngine;

public class DevilAnimationController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastMovement;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        if (_animator == null)
        {
            Debug.LogError("Animator component is missing on the Player GameObject.");
        }
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on the Player GameObject.");
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        // Get movement from PlayerController
        Vector3 movement = GetComponent<PlayerController>().Movement;

        if (movement.magnitude > 0)
        {
            _lastMovement = movement;
        }

        UpdateAnimation(movement);
    }

    private void UpdateAnimation(Vector3 movement)
    {
        float moveX = movement.x;
        float moveZ = movement.z;

        if (moveZ > 0)
        {
            _animator.Play("devil cima"); // Facing up
        }
        else if (moveZ < 0)
        {
            _animator.Play("devil idle"); // Facing down (idle)
        }
        else if (Mathf.Abs(moveX) > 0)
        {
            _animator.Play("devil side"); // Facing right
            _spriteRenderer.flipX = moveX < 0; // Flip only the sprite, not the entire object
        }
        else
        {
            _animator.Play("devil idle"); // Default to idle
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_lastMovement);
        }
        else
        {
            _lastMovement = (Vector3)stream.ReceiveNext();
            UpdateAnimation(_lastMovement);
        }
    }
}
