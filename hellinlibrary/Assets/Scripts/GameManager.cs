using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [SerializeField] GameObject devilPrefab; // Reference to Devil prefab
    [SerializeField] Transform devilSpawnerPosition; // Spawn position for Devil

    [SerializeField] GameObject humanPrefab; // Reference to Human prefab
    [SerializeField] Transform humanSpawnerPosition; // Spawn position for Human

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Spawn Devil for the room creator
                PhotonNetwork.Instantiate("Prefabs/" + devilPrefab.name, devilSpawnerPosition.position, Quaternion.identity);
            }
            else
            {
                // Spawn Human for other players
                PhotonNetwork.Instantiate("Prefabs/" + humanPrefab.name, humanSpawnerPosition.position, Quaternion.identity);
            }
        }
    }
}