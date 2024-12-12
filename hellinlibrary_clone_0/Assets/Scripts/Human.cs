using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Human : Character, IPunObservable
{
    public Slider healthBar;
    private float maxHealth = 100f;
    private float currentHealth;
    private int totalHumanDeaths;

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        UpdateHealthUi();
    }

    public override void TakeDamage(float damage)
    {
        if (!PhotonNetwork.IsMasterClient) return; // Ensure damage calculations happen only on the master client

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUi();

        if (currentHealth <= 0.01f)
        {
            Debug.Log("Human has died!");
            photonView.RPC("HandleDeath", RpcTarget.MasterClient);
        }

        // Synchronize the health update across clients
        photonView.RPC("UpdateHealthUiRPC", RpcTarget.All, currentHealth);
    }

    [PunRPC]
    private void UpdateHealthUiRPC(float updatedHealth)
    {
        currentHealth = updatedHealth;
        UpdateHealthUi();
    }
    private void UpdateHealthUi()
    {
        healthBar.value = currentHealth / maxHealth;

        if (currentHealth >= 75)
            healthBar.fillRect.GetComponent<Image>().color = Color.green;
        else if (currentHealth >= 50)
            healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        else
            healthBar.fillRect.GetComponent<Image>().color = Color.red;
    }

    [PunRPC]
    private void HandleDeath()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        totalHumanDeaths += 1;

        Debug.Log($"Human was killed. Total Humans killed: {totalHumanDeaths}");
        photonView.RPC("SyncDeathsRPC", RpcTarget.All, totalHumanDeaths);

        if (totalHumanDeaths >= 4)
        {
            Debug.Log("4 humans killed! DEVIL WINS THE GAME");
            photonView.RPC("AnnounceDevilVictory", RpcTarget.All);
        }
        else
        {
            StartCoroutine(HumanRespawn());
        }
    }

    [PunRPC]
    private void SyncDeathsRPC(int updatedDeaths)
    {
        totalHumanDeaths = updatedDeaths;
    }

    [PunRPC]
    private void AnnounceDevilVictory()
    {
        // Handle Devil victory logic
        Debug.Log("Devil wins! All clients notified.");
        GameManager.Instance.EndGame("Devil Wins!");
    }

    [PunRPC]
    private IEnumerator RespawnHumanRPC(Vector3 respawnPosition)
    {
        gameObject.SetActive(false);

        yield return new WaitForSeconds(3);

        transform.position = respawnPosition; // Update position on all clients
        gameObject.SetActive(true);

        currentHealth = maxHealth;
        UpdateHealthUi();

        photonView.RPC("UpdateHealthUiRPC", RpcTarget.All, currentHealth);
    }

    private IEnumerator HumanRespawn()
    {
        Vector3 respawnPosition = GetRespawnPosition(); // You can customize this function to get a valid respawn position
        photonView.RPC("RespawnHumanRPC", RpcTarget.All, respawnPosition);
        yield return null; // The actual logic runs in RespawnHumanRPC
    }

    private Vector3 GetRespawnPosition()
    {
        // Example respawn logic, could be replaced with a spawn point system
        return new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            TakeDamage(25f);
            Debug.Log("Human took 25 damage");
        }
    }

    void Update()
    {
        // Any additional per-frame logic
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other clients
            stream.SendNext(currentHealth);
            stream.SendNext(totalHumanDeaths);
        }
        else
        {
            // Receive data from other clients
            currentHealth = (float)stream.ReceiveNext();
            totalHumanDeaths = (int)stream.ReceiveNext();

            // Update UI after receiving updated health
            UpdateHealthUi();
        }
    }
}
