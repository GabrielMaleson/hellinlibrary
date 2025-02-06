using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Devil : Character, IPunObservable
{
    public Human human;
    public float attackCooldown = 2.0f;
    private bool canAttack = true;
    private PlayerController playerController;
    public GameObject clawHitBox;

    private PhotonView photonView;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        photonView = GetComponent<PhotonView>();
    }

    public void Attack(Human target)
    {
        if (canAttack)
        {
            // Activate claw hitbox and handle cooldown
            StartCoroutine(ClawAttack());
        }
    }

    private IEnumerator ClawAttack()
    {
        canAttack = false;

        // Always move the attack forward
        Vector3 spawnPosition = transform.position + (transform.forward * 1.5f) + (transform.up * -0.2f);

        clawHitBox.transform.position = spawnPosition;

        clawHitBox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        clawHitBox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void ApplySpeedDebuff(float factor, float duration)
    {
        GameManager.Instance.RunCoroutine(SpeedDebuffRoutine(factor, duration));
    }

    private IEnumerator SpeedDebuffRoutine(float factor, float duration)
    {
        float originalSpeed = playerController.speed;
        playerController.speed *= factor;

        yield return new WaitForSeconds(duration);

        playerController.speed = originalSpeed;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canAttack)
            {
                ApplySpeedDebuff(0.5f, attackCooldown);
                photonView.RPC("AttackRPC", RpcTarget.All);
            }
        }
      
    }

    [PunRPC]
    public void AttackRPC()
    {
        Attack(human);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(canAttack);
        }
        else
        {
            canAttack = (bool)stream.ReceiveNext();
        }
    }
}