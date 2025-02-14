using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using System.Collections.Generic;

public class BookSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bookPrefab; // Prefab do livro
    public Tilemap tilemap; // Referência ao Tilemap
    public LayerMask wallLayer; // Layer que contém os objetos WallCollision

    [Header("Spawn Settings")]
    public int numberOfBooks = 10; // Quantidade de livros para spawnar
    public float spawnHeightOffset = 0.2f; // Altura acima do chão para spawnar os livros

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
            Debug.LogError("Book prefab ou Tilemap não estão atribuídos.");
            return;
        }

        // Obtém os limites do Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        for (int i = 0; i < numberOfBooks; i++)
        {
            Vector3 spawnPosition = GetRandomPositionWithinBounds(bounds);
            spawnPosition.y += spawnHeightOffset; // Ajusta a altura para ficar acima do chão

            // Verifica se o spawn está colidindo com algo
            if (!IsPositionBlocked(spawnPosition))
            {
                GameObject book = PhotonNetwork.Instantiate(bookPrefab.name, spawnPosition, Quaternion.Euler(0, 270, 0)); // Ajusta a orientação
                book.name = "Book";
                spawnedBooks.Add(book);
            }
            else
            {
                i--; // Tenta novamente se a posição estiver bloqueada
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
        worldPosition.y = tilemap.transform.position.y; // Mantém a altura da base do tilemap

        return worldPosition;
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        // Checa se há colisões na posição especificada
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f, wallLayer); // 0.5f é o raio da verificação
        return colliders.Length > 0; // Se houver qualquer colisão, a posição está bloqueada
    }
}
