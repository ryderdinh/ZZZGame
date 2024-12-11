using System;
using System.Collections;
using System.Collections.Generic;
using DTT.Networking.ConnectionStatus;
using UnityEngine;

/// <summary>
/// Listens and responds to status updates of internet status targets.
/// </summary>
internal class InternetSensitiveBehaviour : MonoBehaviour
{
    /// <summary>
    /// Executes code if the default internet status target its status is online.
    /// </summary>
    private void Awake()
    {
        // Add a callback to the default target's online status which will be instant if we are already online.
        InternetStatusManager.DefaultTarget.On(InternetStatus.ONLINE, () =>
        {
            /* Code to execute if we are online. */
        });
    }

    /// <summary>
    /// Retrieves a custom target from the internet status manager, tries to attempt a reconnect if
    /// it is online and executes some code if the attempt failed. Also starts listening for status updates.
    /// </summary>
    private void Start()
    {
        // Retrieve a custom target added to the window named 'OpenDNS'.
        InternetStatusTarget openDns = InternetStatusManager.GetTarget("OpenDNS");
        if (openDns == null)
            return;

        if (openDns.IsOffline)
        {
            // Attempt a reconnect of at least 5 seconds before calling back with a result.
            openDns.AttemptReconnect(5f, (bool success) =>
            {
                if (!success)
                {
                    /* Code to execute if our attempted reconnect failed. */
                }
            });
        }
        
        openDns.StatusUpdate += (InternetStatus newStatus) =>
        {
            /* Code to execute based on the new status of the 'OpenDNS' target. */
        };
    }
}
