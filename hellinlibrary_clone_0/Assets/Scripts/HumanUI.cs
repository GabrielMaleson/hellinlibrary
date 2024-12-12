using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HumanUI : Character, IPunObservable
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
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthUi();

            if (currentHealth <= 0.01f)
            {
                Debug.Log("Human has died!");
                photonView.RPC("RespawnHumanRPC", RpcTarget.All);
            }
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

    [PunRPC]
    public void RespawnHumanRPC()
    {
        StartCoroutine(HumanRespawn());
    }

    private IEnumerator HumanRespawn()
    {
        gameObject.SetActive(false);

        if (photonView.IsMine)
        {
            totalHumanDeaths += 1;

            if (totalHumanDeaths == 4)
            {
                Debug.Log("4 humans killed! DEVIL WINS THE GAME");
            }
            else
            {
                Debug.Log($"Human was killed. Total Humans killed: {totalHumanDeaths}");
            }
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
            if (photonView.IsMine)
            {
                TakeDamage(25f);
                Debug.Log("Human took 25 damage");
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(totalHumanDeaths);
        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();
            totalHumanDeaths = (int)stream.ReceiveNext();
            UpdateHealthUi();
        }
    }
}
