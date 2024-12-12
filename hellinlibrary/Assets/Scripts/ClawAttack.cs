using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Required for Photon Networking

public class ClawAttack : MonoBehaviour, IPunObservable
{
    private Devil devil;
    private PhotonView photonView; // Reference to PhotonView

    // Example state to sync (e.g., damage applied or other state variables)
    private float syncedDamage;

    // Update is called once per frame
    void Update()
    {
        // Example: Update synced state
        if (photonView.IsMine)
        {
            // Simulate some logic for demonstration
            syncedDamage = 25f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView != null && !photonView.IsMine)
        {
            // Exit if this object is not owned by the local player
            return;
        }

        if (other.CompareTag("Human"))
        {
            // Try to get the HumanUI component
            HumanUI target = other.GetComponent<HumanUI>();

            if (target != null)
            {
                // Apply damage and sync state
                target.TakeDamage(syncedDamage);
                Debug.Log($"Human took {syncedDamage} dmg");
            }
        }
    }

    // Synchronize data across the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to others
            stream.SendNext(syncedDamage);
        }
        else
        {
            // Receive data from others
            syncedDamage = (float)stream.ReceiveNext();
        }
    }
}
