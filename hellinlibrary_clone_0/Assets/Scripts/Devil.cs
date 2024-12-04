using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Character
{ 
    public Human human;
    public float attackCooldown = 2.0f;
    private bool canAttack = true;
    private PlayerController playerController;
    public GameObject clawHitbox;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
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
        clawHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        clawHitbox.SetActive(false);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && canAttack)
        {
            Attack(human);
        }
    }

        
}

