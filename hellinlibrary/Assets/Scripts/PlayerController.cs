using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Fields

    public static GameObject LocalPlayerInstance;
    private Rigidbody _rb;
    private TMP_Text _namePlayer;
    [SerializeField] private float _playerSpeed = 10f;
    private Vector3 networkPosition;
    private string _nickname;

    #endregion

    #region Properties

    public Vector3 Movement { get; set; }
    
    public float speed
    {
        get => _playerSpeed;
        set => _playerSpeed = value;
    }

    #endregion

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
    }

    private void Start()
    {
        // Assign Rigidbody
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the Player GameObject.");
            return;
        }

        // Assign TMP_Text
        _namePlayer = GetComponentInChildren<TMP_Text>();
        if (_namePlayer == null)
        {
            Debug.LogError("TMP_Text component is missing in the Player GameObject's children.");
            return;
        }

        // Set nickname
        _nickname = photonView.IsMine ? PhotonNetwork.LocalPlayer.NickName : _nickname;
        _namePlayer.text = _nickname;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        // Handle player input
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        float jump = _rb.velocity.y;

        Movement = new Vector3(moveH * speed, jump, moveV * speed);
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Local player
            _rb.velocity = Movement;
        }
        else
        {
            // Network player interpolation
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other clients
            stream.SendNext(transform.position);
            stream.SendNext(_nickname);
        }
        else
        {
            // Receive data from other clients
            networkPosition = (Vector3)stream.ReceiveNext();
            _nickname = (string)stream.ReceiveNext();

            // Update name display
            _namePlayer.text = _nickname;
        }
    }
}
