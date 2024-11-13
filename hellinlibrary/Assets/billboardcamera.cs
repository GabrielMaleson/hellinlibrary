using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardcamera : MonoBehaviour
{
    // Start is called before the first frame update
    Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
