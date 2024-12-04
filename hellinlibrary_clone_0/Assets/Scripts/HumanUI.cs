using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanUI : Character
{
    public Slider healthBar;
    private float maxHealth = 100f;
    private float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUi();
    }
   
    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUi();

        if(currentHealth <= 0.01f)
        {
            Debug.Log("Human has died!");
            Destroy(gameObject);
        }

    }
    
    private void UpdateHealthUi()
    {
        healthBar.value = currentHealth / maxHealth;

        if (currentHealth >= 75 )
            healthBar.fillRect.GetComponent<Image>().color = Color.green;
        else if (currentHealth >= 50)
            healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        else
            healthBar.fillRect.GetComponent<Image>().color = Color.red;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
