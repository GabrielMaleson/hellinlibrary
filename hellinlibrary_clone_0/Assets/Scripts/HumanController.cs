using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class HumanController : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxBooks = 5;
    private int currentBooks = 0;
    private int deliveredBooks;
    public float baseSpeed = 5.0f;
    public float speedReductionPerBook = 0.5f;
    public float humanSpeed;

    private float dropHoldTime = 0f;
    private bool hasDroppedAllBooks = false;

    private float stackHeight = -0.5f; // Current stack height of books
    private float stackIncrement = 0.05f; // Vertical increment for each book

    private Vector3 lastPosition;

    private List<GameObject> carriedBooks = new List<GameObject>();

    public float pickupRange = 2.0f;
    private GameObject nearestBook;

    private bool nearFallen = false;
    public GameObject fallenPrefab;

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
    public float PlayerSpeed
    {
        get => _playerSpeed;
        set => _playerSpeed = value;
    }

    #endregion

    private void UpdateSpeed()
    {
        switch (currentBooks)
        {
            case 1:
                humanSpeed = baseSpeed;
                break;
            case 2:
                humanSpeed = baseSpeed * 0.95f;
                break;
            case 3:
                humanSpeed = baseSpeed * 0.90f;
                break;
            case 4:
                humanSpeed = baseSpeed * 0.80f;
                break;
            case 5:
                humanSpeed = baseSpeed * 0.65f;
                break;
            default:
                humanSpeed = baseSpeed;
                break;
        }
    }

    private void FindNearestBook()
    {
        nearestBook = null;
        float closestDistance = float.MaxValue;

        GameObject[] books = GameObject.FindGameObjectsWithTag("Book");
        foreach (GameObject book in books)
        {
            if (book == null) continue;

            float distance = Vector3.Distance(transform.position, book.transform.position);

            if (distance <= pickupRange && distance < closestDistance)
            {
                PhotonView bookPhotonView = book.GetComponent<PhotonView>();
                if (bookPhotonView != null) // Ensure PhotonView exists
                {
                    closestDistance = distance;
                    nearestBook = book;
                }
            }
        }
    }

    public void PickUpBook(GameObject book)
    {
        if (book == null)
        {
            Debug.LogWarning("Attempted to pick up a book, but the book is null.");
            return;
        }

        PhotonView bookPhotonView = book.GetComponent<PhotonView>();
        if (bookPhotonView == null)
        {
            Debug.LogWarning($"The book {book.name} does not have a PhotonView component.");
            return;
        }

        if (currentBooks < maxBooks)
        {
            photonView.RPC(nameof(RPC_PickUpBook), RpcTarget.AllBuffered, bookPhotonView.ViewID);
        }
        else
        {
            Debug.Log("Cannot pick up more books. Max capacity reached.");
        }
    }

    [PunRPC]
    private void RPC_PickUpBook(int bookViewID)
    {
        GameObject book = PhotonView.Find(bookViewID).gameObject;

        carriedBooks.Add(book);
        currentBooks++;
        UpdateSpeed();

        book.SetActive(false);
    }

    private void DeliverBooks()
    {
        if (currentBooks > 0)
        {
            Debug.Log($"Attempting to deliver {currentBooks} books..."); // Debugging
            photonView.RPC(nameof(RPC_DeliverBooks), RpcTarget.AllBuffered, currentBooks);
        }
    }

    [PunRPC]
    private void RPC_DeliverBooks(int booksToDeliver)
    {
        if (booksToDeliver <= 0) return;

        deliveredBooks += booksToDeliver;

        string message = booksToDeliver == 1
            ? $"Delivered {booksToDeliver} book to Fallen!"
            : $"Delivered {booksToDeliver} books to Fallen!";

        Debug.Log(message); // Local log for debugging
        photonView.RPC(nameof(RPC_LogMessage), RpcTarget.All, message);

        if (deliveredBooks >= 10)
        {
            photonView.RPC(nameof(RPC_LogMessage), RpcTarget.All,
                $"Delivered {deliveredBooks} books to Fallen! You are FREED from the curse!");
        }
        else
        {
            photonView.RPC(nameof(RPC_LogMessage), RpcTarget.All,
                $"Delivered {deliveredBooks} books so far to Fallen!");
        }

        currentBooks = 0;
        carriedBooks.Clear();
        UpdateSpeed();
    }

    [PunRPC]
    private void RPC_LogMessage(string message)
    {
        Debug.Log(message);
    }

    public void DropBook()
    {
        if (currentBooks > 0)
        {
            GameObject bookToDrop = carriedBooks[carriedBooks.Count - 1];
            carriedBooks.Remove(bookToDrop);

            photonView.RPC(nameof(RPC_DropBook), RpcTarget.AllBuffered, bookToDrop.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void RPC_DropBook(int bookViewID)
    {
        GameObject book = PhotonView.Find(bookViewID).gameObject;

        currentBooks--;
        UpdateSpeed();

        book.SetActive(true);
        book.transform.position = transform.position + new Vector3(0, stackHeight, 0);
        stackHeight += stackIncrement;
    }

    public void DropAllBooks()
    {
        stackHeight = -0.5f;
        while (currentBooks > 0)
        {
            DropBook();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Book"))
        {
            nearestBook = other.gameObject;
        }

        if (other.CompareTag("Fallen")) // Use tag to detect Fallen
        {
            nearFallen = true;
            Debug.Log("Near Fallen object"); // Debugging
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Book"))
        {
            nearestBook = null;
        }

        if (other.CompareTag("Fallen"))
        {
            nearFallen = false;
            dropHoldTime = 0f;
        }
    }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _namePlayer = GetComponentInChildren<TMP_Text>();

        _nickname = photonView.IsMine ? PhotonNetwork.LocalPlayer.NickName : _nickname;
        _namePlayer.text = _nickname;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        float jump = _rb.velocity.y;

        Movement = new Vector3(moveH * PlayerSpeed, jump, moveV * PlayerSpeed);
        FindNearestBook();

        if (Input.GetKeyDown(KeyCode.Space) && nearestBook != null)
        {
            PickUpBook(nearestBook);
        }

        if (nearFallen && currentBooks > 0)
        {
            if (Input.GetKey(KeyCode.E))
            {
                dropHoldTime += Time.deltaTime;
                if (dropHoldTime >= 1.0f)
                {
                    DeliverBooks();
                    dropHoldTime = 0f;
                }
            }
            else
            {
                dropHoldTime = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.Q) && currentBooks > 0)
        {
            dropHoldTime += Time.deltaTime;

            if (dropHoldTime > 1.0f && !hasDroppedAllBooks)
            {
                DropAllBooks();
                dropHoldTime = 0f;
                hasDroppedAllBooks = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!hasDroppedAllBooks && dropHoldTime <= 1.0f && currentBooks > 0)
            {
                DropBook();
            }

            dropHoldTime = 0f;
            hasDroppedAllBooks = false;
        }

        if (transform.position != lastPosition)
        {
            stackHeight = -0.5f;
        }
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            _rb.velocity = Movement;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(_nickname);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            _nickname = (string)stream.ReceiveNext();
        }
    }
}
