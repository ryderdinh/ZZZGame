#if UNITY_EDITOR

using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// The internet status types used in the internet status window.
    /// </summary>
    internal enum EditorInternetStatus
    {
        /// <summary>
        /// There is no connection.
        /// </summary>
        [InspectorName("Offline")]
        OFFLINE = 1,

        /// <summary>
        /// Reconnecting.
        /// </summary>
        [InspectorName("Reconnecting")]
        RECONNECTING = 2,

        /// <summary>
        /// There is a connection.
        /// </summary>
        [InspectorName("Online")]
        ONLINE = 3
    }
}

#endif