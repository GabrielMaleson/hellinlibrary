using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Human : Character, IPunObservable
{
    public Slider healthBar;
    private float maxHealth = 100f;
    private float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUi();
    }

    public override void TakeDamage(float damage)
    {
        if (!PhotonNetwork.IsMasterClient) return;

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
        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.RunCoroutine(HumanRespawn());
    }

    private IEnumerator HumanRespawn()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        gameObject.SetActive(true);

        currentHealth = maxHealth;
        UpdateHealthUi();
    }

    void Update() { }

    // Synchronize data with Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other clients
            stream.SendNext(currentHealth);
        }
        else
        {
            // Receive data from the master client
            currentHealth = (float)stream.ReceiveNext();
        }
        UpdateHealthUi();
    }
}
