using UnityEngine;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// The internet status types.
    /// </summary>
    public enum InternetStatus
    {
        /// <summary>
        /// No status has been determined yet.
        /// </summary>
        [InspectorName("Unknown")]
        UNKNOWN = 0,

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
