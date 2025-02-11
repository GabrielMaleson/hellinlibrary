using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class BookSpawner : MonoBehaviour, IPunObservable
{
    [Header("References")]
    public GameObject bookPrefab; // Prefab of the Book to instantiate

    [Header("Spawn Settings")]
    public int numberOfBooks = 10; // Number of books to spawn

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
        if (bookPrefab == null)
        {
            Debug.LogError("Book prefab is not assigned.");
            return;
        }

        // Find all GameObjects titled "Collision" in the Grid -> TestTilemap hierarchy
        GameObject[] collisionObjects = GameObject.FindGameObjectsWithTag("Collision");

        if (collisionObjects.Length == 0)
        {
            Debug.LogError("No GameObjects with the name 'Collision' found in the hierarchy.");
            return;
        }

        for (int i = 0; i < numberOfBooks; i++)
        {
            // Select a random "Collision" GameObject
            GameObject collisionObject = collisionObjects[Random.Range(0, collisionObjects.Length)];

            // Get the position of the "Collision" GameObject
            Vector3 spawnPosition = collisionObject.transform.position;

            // Spawn the book slightly above the "Collision" GameObject
            spawnPosition.y += 0.1f;

            // Instantiate the book
            GameObject book = PhotonNetwork.Instantiate(bookPrefab.name, spawnPosition, Quaternion.identity);

            book.name = "Book"; // Rename the book
            spawnedBooks.Add(book);
        }
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