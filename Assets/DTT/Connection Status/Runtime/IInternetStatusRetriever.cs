using System;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Provides an interface for retrieving status of an internet target.
    /// </summary>
    public interface IInternetStatusRetriever
    {
        /// <summary>
        /// Should attempt a reconnect for a default reconnect duration.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        void AttemptReconnect(Action<bool> callback);

        /// <summary>
        /// Should attempt a reconnect for a default reconnect duration.
        /// </summary>
        void AttemptReconnect();

        /// <summary>
        /// Should attempt a reconnect for at least the given minimum duration before a callback is given.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        /// <param name="minDuration">The minimum duration the reconnect should at least take.</param>
        void AttemptReconnect(float minDuration, Action<bool> callback);

        /// <summary>
        /// Should subscribe a one time callback on given status. Callback will be instant
        /// if the statusOn is the same as the current status.
        /// </summary>
        /// <param name="statusOn">The status to callback on.</param>
        /// <param name="callback">The callback.</param>
        void On(InternetStatus statusOn, Action callback);
    }
}
