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
}
