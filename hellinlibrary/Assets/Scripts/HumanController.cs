using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public int maxBooks = 5;
    private int currentBooks = 0;
    public float baseSpeed = 5.0f;
    public float speedReductionPerBook = 0.5f;
    public float humanSpeed;

    private float dropHoldTime = 0f;
    private bool hasDroppedAllBooks = false;

    private float stackHeight = -0.5f; // Altura atual da pilha de livros
    private float stackIncrement = 0.05f; // Incremento vertical para cada livro

    private Vector3 lastPosition;

    private List<GameObject> carriedBooks = new List<GameObject>();

    public float pickupRange = 2.0f;
    private GameObject nearestBook;

    private bool nearFallen = false;
    public GameObject fallenPrefab;


    void Start()
   {


      humanSpeed = baseSpeed;
   }

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
        float closestDistance = float.MaxValue; // We need to compare distances between the player and each book. Starting with an arbitrarily large value ensures that the first book we find will automatically become the "closest."

        GameObject[] books = GameObject.FindGameObjectsWithTag("Book");
        foreach (GameObject book in books)
        {
            float distance = Vector3.Distance(transform.position, book.transform.position); // distance between player and book

            if (distance <= pickupRange && distance < closestDistance) // Check if the book is within range
            {
                closestDistance = distance;
                nearestBook = book; // Update the nearest book
            }
        }


    }

    public void PickUpBook(GameObject book)
   {
        if (currentBooks < maxBooks) // Only pick up if haven't reached maxBooks
        {
            carriedBooks.Add(book);
            currentBooks++;

            UpdateSpeed();

            book.SetActive(false);

        }
   }

    private void DeliverBooks()
    {
        Debug.Log($"Delivered {currentBooks} books to Fallen!");

        // Zera os livros carregados
        currentBooks = 0;
        carriedBooks.Clear();

        // Atualiza a velocidade para o estado sem livros
        UpdateSpeed();
    }

    public void DropBook()
   {
      if (currentBooks > 0) // Only drop if carrying at least one book
      {    
         GameObject bookToDrop = carriedBooks[carriedBooks.Count - 1];

         carriedBooks.Remove(bookToDrop);
         currentBooks--;
         UpdateSpeed();

         bookToDrop.SetActive(true);
         bookToDrop.transform.position = transform.position + new Vector3(0, stackHeight, 0); //

         stackHeight += stackIncrement; // posicionamento do livro muda quanto mais livros forem dropados no mesmo spot

        }
   }
    
    public void DropAllBooks()
    {
        stackHeight = -0.5f; // zera o posicionamento do livro

        while (currentBooks > 0)
        {
            DropBook(); // dropa todos os livros ate livros = 0
        }
       
        stackHeight = -0.5f;
    }


   private void OnTriggerEnter(Collider other)
   {    
        if (other.CompareTag("Book"))
        {
            nearestBook = other.gameObject;
           
        }

        if (other.gameObject == fallenPrefab)
        {
            nearFallen = true; // Enter fallen radius
        }
   }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Book"))
        {
            nearestBook = null; // Clear the reference to the nearby book
        }
        if (other.gameObject == fallenPrefab)
        {
            nearFallen = false;
            dropHoldTime = 0f; // Leaves fallen radius
        }
    }

    private void Update()
    {
        FindNearestBook();

        if (Input.GetKeyDown(KeyCode.Space) && nearestBook != null)
        {
            PickUpBook(nearestBook);
        }

        if (nearFallen && currentBooks > 0)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                dropHoldTime += Time.deltaTime; // Incrementa o tempo de hold

                if (dropHoldTime >= 1.0f) // tempo de entrega
                {
                    DeliverBooks();
                    dropHoldTime = 0f;
                } 
            }
            else
            {
                dropHoldTime = 0f; // impede jogador de quebrar a mecanica de hold
            }
        }
        else if (Input.GetKey(KeyCode.Q) && currentBooks > 0)
        {
            dropHoldTime += Time.deltaTime;


            if (dropHoldTime > 1.0f && !hasDroppedAllBooks)
            {
                DropAllBooks();
                dropHoldTime = 0f; // Reseta o contador
                hasDroppedAllBooks = true;
            }
        }

        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!hasDroppedAllBooks && dropHoldTime <= 1.0f && currentBooks > 0) // Quick release
            {
                DropBook(); // Drop a single book
            }

            // Reset hold time and drop state
            dropHoldTime = 0f;
            hasDroppedAllBooks = false;
        }

        if (transform.position != lastPosition) // If position changes
        {
            stackHeight = -0.5f; // Reset the stack height
        }
        lastPosition = transform.position;

        //talvez mudar esse movimento de lugar
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * humanSpeed * Time.deltaTime * horizontalInput);
        transform.Translate(Vector3.forward * humanSpeed * Time.deltaTime * verticalInput);
    }
}