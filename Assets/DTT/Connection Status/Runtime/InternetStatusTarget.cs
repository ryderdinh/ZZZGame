using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Represents an internet target of which the status can be retrieved.
    /// </summary>
    [Serializable]
    public sealed class InternetStatusTarget : IInternetStatusRetriever
    {
        /// <summary>
        /// The name of the target.
        /// </summary>
        [SerializeField, Tooltip("The name of the target.")]
        private string _name;

        /// <summary>
        /// The current internet status of the target.
        /// </summary>
        [SerializeField, Tooltip("The current internet status of the target.")]
        private InternetStatus _currentStatus;

        /// <summary>
        /// Whether the target uses a status override for testing.
        /// </summary>
        [SerializeField, Tooltip("Whether the target uses a status override for testing.")]
        private bool _usesStatusOverride;

        /// <summary>
        /// The status override value used for testing.
        /// </summary>
        [SerializeField, Tooltip("The status override value used for testing.")]
        private InternetStatus _statusOverride;

        /// <summary>
        /// The address of the target.
        /// </summary>
        [SerializeField, Tooltip("The address of the target.")]
        private string _address;

        /// <summary>
        /// The ping interval. 
        /// </summary>
        [SerializeField, Tooltip("The ping interval. ")]
        private float _pingInterval;

        /// <summary>
        /// The maximum time used for reconnecting to the target.
        /// </summary>
        [SerializeField, Tooltip("The maximum time used for reconnecting to the target.")]
        private float _maxReconnectDuration;

        /// <summary>
        /// Fired when the Internet status has been updated.
        /// </summary>
        public event Action<InternetStatus> StatusUpdate;

        /// <summary>
        /// Fired when the internet status has been updated to online.
        /// </summary>
        public event Action OnOnline;

        /// <summary>
        /// Fired when the internet status has been updated to offline.
        /// </summary>
        public event Action OnOffline;
        
        /// <summary>
        /// Whether we are currently not connected.
        /// </summary>
        public bool IsOffline => CurrentStatus == InternetStatus.OFFLINE;

        /// <summary>
        /// Whether we are currently connected.
        /// </summary>
        public bool IsOnline => CurrentStatus == InternetStatus.ONLINE;
        
        /// <summary>
        /// The name of the target.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Whether the target uses a status override for testing.
        /// </summary>
        public bool UsesStatusOverride => _usesStatusOverride;

        /// <summary>
        /// The status override value used for testing.
        /// </summary>
        public InternetStatus StatusOverride => _statusOverride;

        /// <summary>
        /// The address of the target.
        /// </summary>
        public string Address => _address;

        /// <summary>
        /// The ping interval. 
        /// </summary>
        public float PingInterval => _pingInterval;

        /// <summary>
        /// The maximum time used for reconnecting to the target.
        /// </summary>
        public float MaxReconnectDuration => _maxReconnectDuration;

        /// <summary>
        /// The current internet status of the target.
        /// </summary>
        public InternetStatus CurrentStatus => _currentStatus;

        /// <summary>
        /// The name of the default target.
        /// </summary>
        public const string DEFAULT_NAME = "Google";

        /// <summary>
        /// The address of the default target.
        /// </summary>
        public const string DEFAULT_ADDRESS = "8.8.8.8";

        /// <summary>
        /// The default ping interval.
        /// </summary>
        public const float DEFAULT_PING_INTERVAL = 1f;

        /// <summary>
        /// The default maximum time used for reconnecting to the target.
        /// </summary>
        public const float DEFAULT_MAX_RECONNECT_DURATION = 10f;

        /// <summary>
        /// The status retriever used by the target.
        /// </summary>
        private IInternetStatusRetriever _statusRetriever;
        
        /// <summary>
        /// Attempts a reconnect for the default reconnect duration.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        public void AttemptReconnect(Action<bool> callback) => _statusRetriever.AttemptReconnect(callback);

        /// <summary>
        /// Attempts a reconnect for the default reconnect duration.
        /// </summary>
        public void AttemptReconnect() => _statusRetriever.AttemptReconnect();

        /// <summary>
        /// Attempts a reconnect for at least the given minimum duration before a callback is given.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        /// <param name="minDuration">The minimum duration the reconnect should at least take.</param>
        public void AttemptReconnect(float minDuration, Action<bool> callback) =>
            _statusRetriever.AttemptReconnect(minDuration, callback);

        /// <summary>
        /// Subscribes a one time callback on given status. Callback will be instant
        /// if the statusOn is the same as the current status.
        /// </summary>
        /// <param name="statusOn">The status to callback on.</param>
        /// <param name="callback">The callback.</param>
        public void On(InternetStatus statusOn, Action callback) => _statusRetriever.On(statusOn, callback);
        
        /// <summary>
        /// Initializes the target by creating a status retriever for it.
        /// </summary>
        internal void Initialize() => _statusRetriever = InternetStatusManager.HookRetrieverToTarget(this);

        /// <summary>
        /// Updates the status of the target invoking events if necessary.
        /// </summary>
        /// <param name="newStatus">The new internet status.</param>
        /// <param name="invokeEvents">Whether to invoke events.</param>
        internal void UpdateStatus(InternetStatus newStatus, bool invokeEvents)
        {
            _currentStatus = newStatus;
            
            if(invokeEvents)
                InvokeStatusEvents();
        }

        /// <summary>
        /// Invokes status events using the current status.
        /// </summary>
        internal void InvokeStatusEvents()
        {
            StatusUpdate?.Invoke(CurrentStatus);
            switch (CurrentStatus)
            {
                case InternetStatus.OFFLINE:
                    OnOffline?.Invoke();
                    break;
                case InternetStatus.ONLINE:
                    OnOnline?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Resets the target's state.
        /// </summary>
        internal void Reset() => _currentStatus = InternetStatus.UNKNOWN;
        
        /// <summary>
        /// The standard internet status target used by the handler.
        /// </summary>
        internal static InternetStatusTarget Standard => new InternetStatusTarget()
        {
            _name = DEFAULT_NAME,
            _usesStatusOverride = false,
            _statusOverride = InternetStatus.UNKNOWN,
            _address = DEFAULT_ADDRESS,
            _pingInterval = DEFAULT_PING_INTERVAL,
            _maxReconnectDuration = DEFAULT_MAX_RECONNECT_DURATION
        };
    }
}