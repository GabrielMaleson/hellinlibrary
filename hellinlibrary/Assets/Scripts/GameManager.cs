using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public Vector3 cameraOffset = new Vector3(0, 10, 0);
    public float cameraSmoothSpeed = 0.125f;  
    public static GameManager Instance;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform playerSpawnerPosition;

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
            PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, playerSpawnerPosition.position, Quaternion.identity);
        }

        GameObject cameraObject = new GameObject("Camera");
        Camera camera = cameraObject.AddComponent<Camera>();

        FollowPlayer followScript = cameraObject.AddComponent<FollowPlayer>();  
        followScript.Player = playerPrefab.transform;              
        followScript.offset = cameraOffset;    
        followScript.smoothSpeed = cameraSmoothSpeed;      
        cameraObject.transform.position = playerPrefab.transform.position + cameraOffset;
        cameraObject.transform.SetParent(playerPrefab.transform);        
    }
}
