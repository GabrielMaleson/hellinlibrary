using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class BookSpawner : MonoBehaviour, IPunObservable
{
    [Header("References")]
    public GameObject bookPrefab; // Prefab of the Book to instantiate
    public GameObject sinkholeChao; // Reference to the SinkholeChao surface

    [Header("Spawn Settings")]
    public int numberOfBooks = 10; // Number of books to spawn
    public float spawnRadius = 20f; // Radius around the SinkholeChao for spawning books

    private List<GameObject> spawnedBooks = new List<GameObject>();

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateBooks();
        }
    }

    private void GenerateBooks()
    {
        if (bookPrefab == null || sinkholeChao == null)
        {
            Debug.LogError("Book prefab or SinkholeChao is not assigned.");
            return;
        }

        for (int i = 0; i < numberOfBooks; i++)
        {
            Vector3 spawnPosition = GetRandomPointAroundSurface(sinkholeChao.transform.position, spawnRadius);
            GameObject Book = PhotonNetwork.Instantiate(bookPrefab.name, spawnPosition, Quaternion.identity);

            Book.name = "Book"; // Rename the book
            spawnedBooks.Add(Book);
        }
    }

    private Vector3 GetRandomPointAroundSurface(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, 0.1f * Mathf.PI);
        float distance = Random.Range(0f, radius);

        Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
        Vector3 spawnPosition = center + offset;

        // Ensure the position is slightly above the surface
        spawnPosition.y = sinkholeChao.transform.position.y + 0.1f;

        return spawnPosition;
    }

    // Sync object state
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Master client sends the data
            stream.SendNext(spawnedBooks.Count);
            foreach (var book in spawnedBooks)
            {
                if (book != null)
                {
                    stream.SendNext(book.transform.position);
                }
            }
        }
        else
        {
            // Other clients receive the data
            int bookCount = (int)stream.ReceiveNext();

            for (int i = 0; i < bookCount; i++)
            {
                if (spawnedBooks.Count <= i)
                {
                    Vector3 position = (Vector3)stream.ReceiveNext();
                    GameObject book = Instantiate(bookPrefab, position, Quaternion.identity);
                    book.name = $"book_{i}";
                    spawnedBooks.Add(book);
                }
                else
                {
                    spawnedBooks[i].transform.position = (Vector3)stream.ReceiveNext();
                }
            }
        }
    }
}
