using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Character
{
    private HumanController humanController;

    void Start()
    {
        humanController = GetComponent<HumanController>();
    }
    public void ApplySpeedBoost(float factor, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(factor, duration));
    }
    private IEnumerator SpeedBoostCoroutine(float factor, float duration)
    {
        float originalSpeed = humanController.humanSpeed;
        humanController.humanSpeed += factor;
        yield return new WaitForSeconds(duration);
        humanController.humanSpeed = originalSpeed;
    }
        
}
