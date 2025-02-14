using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using System.Collections.Generic;

public class BookSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bookPrefab; // Prefab do livro
    public Tilemap tilemap; // Refer�ncia ao Tilemap
    public LayerMask wallLayer; // Layer que cont�m os objetos WallCollision

    [Header("Spawn Settings")]
    public int numberOfBooks = 10; // Quantidade de livros para spawnar
    public float spawnHeightOffset = 0.2f; // Altura acima do ch�o para spawnar os livros

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
        if (bookPrefab == null || tilemap == null)
        {
            Debug.LogError("Book prefab ou Tilemap n�o est�o atribu�dos.");
            return;
        }

        // Obt�m os limites do Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        for (int i = 0; i < numberOfBooks; i++)
        {
            Vector3 spawnPosition = GetRandomPositionWithinBounds(bounds);
            spawnPosition.y += spawnHeightOffset; // Ajusta a altura para ficar acima do ch�o

            // Verifica se o spawn est� colidindo com algo
            if (!IsPositionBlocked(spawnPosition))
            {
                GameObject book = PhotonNetwork.Instantiate(bookPrefab.name, spawnPosition, Quaternion.Euler(0, 270, 0)); // Ajusta a orienta��o
                book.name = "Book";
                spawnedBooks.Add(book);
            }
            else
            {
                i--; // Tenta novamente se a posi��o estiver bloqueada
            }
        }
    }

    private Vector3 GetRandomPositionWithinBounds(BoundsInt bounds)
    {
        Vector3Int randomCell = new Vector3Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax),
            Random.Range(bounds.zMin, bounds.zMax)
        );

        Vector3 worldPosition = tilemap.CellToWorld(randomCell);
        worldPosition.y = tilemap.transform.position.y; // Mant�m a altura da base do tilemap

        return worldPosition;
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        // Checa se h� colis�es na posi��o especificada
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f, wallLayer); // 0.5f � o raio da verifica��o
        return colliders.Length > 0; // Se houver qualquer colis�o, a posi��o est� bloqueada
    }
}
