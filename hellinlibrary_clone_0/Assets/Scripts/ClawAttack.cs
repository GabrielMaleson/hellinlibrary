using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawAttack : MonoBehaviour
{
    private Devil devil;
    //public Human human;
    // Start is called before the first frame update
    void Start()
    {
         
        AssignDevilReference();
    
        //human = GetComponent<Human>();
    }

    private void AssignDevilReference()
    {
        // Dynamically fetch the Devil component from the parent or the scene
        devil = GetComponentInParent<Devil>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
         if (devil == null)
        {
            AssignDevilReference(); // Reassign if the reference is lost
        }
        if (other.CompareTag("Human"))
        {
            // Try to get the HumanUI component
            HumanUI target = other.GetComponent<HumanUI>();

            if (target != null)
            {
                // Apply speed effects to Devil and Human    
                // devil.ApplySpeedDebuff(0.2f, 2.0f);
                // target.ApplySpeedBoost(1.5f, 2.0f);
                
                target.TakeDamage(25f);
                Debug.Log("Human took 1 dmg");
            }
        }
    
    }
}
