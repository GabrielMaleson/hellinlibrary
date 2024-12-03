using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using Random = UnityEngine.Random;


public class CriarEConectar : MonoBehaviourPunCallbacks
{
    #region Campos Privados

    [SerializeField] private TMP_InputField _nickname;
    [SerializeField] private TMP_InputField _roomID;
    [SerializeField] private string devilPrefabName = "Devil"; // Prefab name for Devil
    [SerializeField] private string humanPrefabName = "Human"; // Prefab name for Human
    [SerializeField] private Transform spawnPointDevil; // Spawn point for Devil
    [SerializeField] private Transform spawnPointHuman; // Spawn point for Human
    private RoomOptions _options = new RoomOptions();

    #endregion

    #region Metodos Unity

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _options.MaxPlayers = 5;
        _options.IsVisible = true;
        _options.IsOpen = true;
    }

    #endregion

    #region Metodos Publicos

    public string GeraCodigo()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        int digitCount = 6;
        for (int i = 0; i < digitCount; i++)
        {
            code += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        Debug.Log(code);
        return code;
    }
    private void SpawnPlayer(string prefabName, Vector3 position, Quaternion rotation)
{
    PhotonNetwork.Instantiate(prefabName, position, rotation);
    Debug.Log($"Spawned {prefabName} at {position}");
}

    public void CriaSala()
    {
        string roomName = GeraCodigo();

        Debug.Log("SALA CRIADA");

        PhotonNetwork.CreateRoom(roomName, _options);
    }

    public void JoinRoom()
    {
        if (_roomID.text == null)
        {
            return;
        }

        PhotonNetwork.JoinRoom(_roomID.text);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void MudaNome()
    {
        PhotonNetwork.LocalPlayer.NickName = _nickname.text;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully.");
        SpawnPlayer(devilPrefabName, spawnPointDevil.position, spawnPointDevil.rotation);
    }

    #endregion

    #region Callbacks Photon

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.LoadLevel("LobbyGame");

        if (!PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer(humanPrefabName, spawnPointHuman.position, spawnPointHuman.rotation);
        }
    }

    #endregion
}
