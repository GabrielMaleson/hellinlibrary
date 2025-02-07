using UnityEngine;

public class DebugSpawnOrigin : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("PlayerSpawnDevil foi instanciado! Stack trace: " + System.Environment.StackTrace);
    }
}