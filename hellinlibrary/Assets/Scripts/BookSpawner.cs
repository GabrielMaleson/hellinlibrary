using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class BookSpawner : MonoBehaviour, IPunObservable
{
    [Header("References")]
    public GameObject bookPrefab; // Prefab of the Book to instantiate
    public Transform tilemapParent; // Parent object of TestTilemap containing the spawnable objects

    [Header("Spawn Settings")]
    public int numberOfBooks = 10; // Number of books to spawn

    private List<GameObject> spawnedBooks = new List<GameObject>();
    private List<Transform> tileObjects = new List<Transform>();

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CollectTileObjects();
            GenerateBooks();
        }
    }

    private void CollectTileObjects()
    {
        if (tilemapParent == null)
        {
            Debug.LogError("Tilemap parent is not assigned.");
            return;
        }

        foreach (Transform child in tilemapParent)
        {
            tileObjects.Add(child);
        }
    }

    private void GenerateBooks()
    {
        if (bookPrefab == null || tileObjects.Count == 0)
        {
            Debug.LogError("Book prefab is not assigned or no tile objects found.");
            return;
        }

        HashSet<int> usedIndices = new HashSet<int>();

        for (int i = 0; i < numberOfBooks; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, tileObjects.Count);
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);
            Transform tile = tileObjects[randomIndex];
            Vector3 spawnPosition = tile.position + Vector3.up * 0.2f; // Slightly above the tile
            GameObject book = PhotonNetwork.Instantiate(bookPrefab.name, spawnPosition, Quaternion.identity);

            book.name = "Book";
            spawnedBooks.Add(book);
        }
    }

    // Sync object state
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
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
