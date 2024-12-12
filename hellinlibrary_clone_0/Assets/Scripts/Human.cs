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

    void Start()
    {
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
            RespawnHuman();
        }
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

    public void RespawnHuman()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Ensure respawn logic happens only on the master client

        GameManager.Instance.RunCoroutine(HumanRespawn());
    }

    private IEnumerator HumanRespawn()
    {
        gameObject.SetActive(false);

        totalHumanDeaths += 1;

        if (totalHumanDeaths == 4)
        {
            Debug.Log("4 humans killed! DEVIL WINS THE GAME");
        }
        else
        {
            Debug.Log($"Human was killed. Total Humans killed: {totalHumanDeaths}");
        }

        yield return new WaitForSeconds(3);
        gameObject.SetActive(true);

        currentHealth = maxHealth;
        UpdateHealthUi();
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
