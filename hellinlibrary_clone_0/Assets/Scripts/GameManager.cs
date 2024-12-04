using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [SerializeField] private string devilPrefabName = "Devil"; // Prefab name for Devil
    [SerializeField] private string humanPrefabName = "Human"; // Prefab name for Human
    [SerializeField] private Transform spawnPointDevil; // Spawn point for Devil
    [SerializeField] private Transform spawnPointHuman; // Spawn point for Human

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
            if (PhotonNetwork.IsMasterClient)
            {
                // Spawn Devil for the room creator
                PhotonNetwork.Instantiate("Prefabs/" + devilPrefabName, spawnPointDevil.position, Quaternion.identity);
            }
            else
            {
                // Spawn Human for other players
                PhotonNetwork.Instantiate("Prefabs/" + humanPrefabName, spawnPointHuman.position, Quaternion.identity);
            }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully.");
        SpawnPlayer(devilPrefabName, spawnPointDevil.position, spawnPointDevil.rotation);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer(humanPrefabName, spawnPointHuman.position, spawnPointHuman.rotation);
        }
    }

    private void SpawnPlayer(string prefabName, Vector3 position, Quaternion rotation)
    {
        PhotonNetwork.Instantiate(prefabName, position, rotation);
        Debug.Log($"Spawned {prefabName} at {position}");
    }
}