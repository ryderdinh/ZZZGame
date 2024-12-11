using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Is able to retrieve status info for an internet status target.
    /// </summary>
    internal class InternetStatusRetriever : MonoBehaviour, IInternetStatusRetriever
    {
        /// <summary>
        /// The target this retriever is getting the status for.
        /// </summary>
        private InternetStatusTarget _target;

        /// <summary>
        /// The routine which checks the status using a ping.
        /// </summary>
        private Coroutine _statusCheckRoutine;

        /// <summary>
        /// Attempts a reconnect for the default reconnect duration.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        public void AttemptReconnect(Action<bool> callback) => AttemptReconnect(_target.MaxReconnectDuration, callback);

        /// <summary>
        /// Attempts a reconnect for the default reconnect duration.
        /// </summary>
        public void AttemptReconnect() => AttemptReconnect(_target.MaxReconnectDuration, null);

        /// <summary>
        /// Attempts a reconnect for at least the given minimum duration before a callback is given.
        /// </summary>
        /// <param name="callback">A callback on whether the reconnect was successful.</param>
        /// <param name="minDuration">The minimum duration the reconnect should at least take.</param>
        public void AttemptReconnect(float minDuration, Action<bool> callback)
        {
            switch (_target.CurrentStatus)
            {
                case InternetStatus.OFFLINE:
                    ResetStatusCheckRoutine();
                    StartCoroutine(Reconnect(minDuration, callback));
                    break;

                case InternetStatus.ONLINE:
                case InternetStatus.RECONNECTING:
                    StartCoroutine(DelayedCallback(minDuration, () =>
                    {
                        callback?.Invoke(_target.CurrentStatus == InternetStatus.ONLINE);
                    }));
                    break;
            }
        }

        /// <summary>
        /// Subscribes a one time callback on given status. Callback will be instant
        /// if the statusOn is the same as the current status.
        /// </summary>
        /// <param name="statusOn">The status to callback on.</param>
        /// <param name="callback">The callback.</param>
        public void On(InternetStatus statusOn, Action callback)
        {
            // Subscribe one time callback to status update.
            _target.StatusUpdate += OneTimeCallback;

            // Call the one time callback immediately if the
            // statusOn is the same as the current status.
            if (statusOn == _target.CurrentStatus)
                OneTimeCallback(statusOn);

            void OneTimeCallback(InternetStatus status)
            {
                if (status == statusOn)
                {
                    callback?.Invoke();
                    _target.StatusUpdate -= OneTimeCallback;
                }
            }
        }
        
        /// <summary>
        /// Sets the target of the retriever.
        /// </summary>
        /// <param name="target">The internet status target.</param>
        internal void SetTarget(InternetStatusTarget target) => _target = target;

        /// <summary>
        /// Starts checking for status updates and subscribes to the scene loaded event.
        /// </summary>
        private void Start()
        {
            StartStatusCheckRoutine();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Unsubscribes from the scene load event.
        /// </summary>
        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        /// <summary>
        /// Waits for a delay amount in seconds before calling a given function.
        /// </summary>
        /// <param name="delay">The delay in seconds.</param>
        /// <param name="callback">The function to execute after the delay.</param>
        private IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        /// <summary>
        /// Tries reconnecting using given minimum duration.
        /// </summary>
        /// <param name="minDuration">The minimum duration the reconnect has to take.</param>
        /// <param name="callback">The callback to fire after the reconnect attempt has finished.</param>
        private IEnumerator Reconnect(float minDuration, Action<bool> callback)
        {
            UpdateStatus(InternetStatus.RECONNECTING);

            // Save the time at which we start to ping.
            float startTime = Time.time;
            float timeout = _target.MaxReconnectDuration;

            yield return Ping(_target.Address, timeout);

            // If the time it took was shorter than the minimum duration,
            // wait the remaining time.
            float pingTime = Time.time - startTime;
            if (pingTime < minDuration)
                yield return new WaitForSeconds(minDuration - pingTime);

            callback?.Invoke(_target.CurrentStatus == InternetStatus.ONLINE);

            // Set the status check routine to start again
            // after the ping interval time has expired.
            StartStatusCheckRoutine(_target.PingInterval);
        }

        /// <summary>
        /// Checks the internet status at stored ping interval setting.
        /// </summary>
        private IEnumerator CheckForStatus()
        {
            while (true)
            {
                // Save the time at which we start to ping.
                float time = Time.time;

                // Update the status to the status override, if the respective flag is set,
                // otherwise ping the set address using the ping interval as timeout value.
                if (_target.UsesStatusOverride)
                    UpdateStatus(_target.StatusOverride);
                else
                    yield return Ping(_target.Address, _target.PingInterval);

                // Wait for the remaining time before we start another ping.
                float leftOverTime = _target.PingInterval - Mathf.Clamp(Time.time - time, 0f, _target.PingInterval);
                if (leftOverTime != 0f)
                    yield return new WaitForSeconds(leftOverTime);
            }
        }

        /// <summary>
        /// Pings given address, stopping after given timeout.
        /// </summary>
        /// <param name="address">The address to ping.</param>
        /// <param name="timeout">The time after which to stop pinging.</param>
        private IEnumerator Ping(string address, float timeout)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // If there is no reachable internet. Update the online status to offline.
                UpdateStatus(InternetStatus.OFFLINE);
                yield break;
            }
            
            float startTime = Time.time;

            Ping ping = new Ping(address);
            while (!ping.isDone)
            {
                yield return null;

                if (Time.time - startTime > timeout)
                    break;
            }

            // Update the online status based on the ping result.
            UpdateStatus(ping.isDone ? InternetStatus.ONLINE : InternetStatus.OFFLINE);

            ping.DestroyPing();
        }

        /// <summary>
        /// Updates the internet status firing the status update event if it has changed.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        private void UpdateStatus(InternetStatus newStatus)
        {
            // Invoke status events if the status has actually changed.
            bool invokeStatusEvents = newStatus != _target.CurrentStatus;

            _target.UpdateStatus(newStatus, invokeStatusEvents);
        }

        /// <summary>
        /// Resets the status check routine.
        /// </summary>
        private void ResetStatusCheckRoutine()
        {
            if (_statusCheckRoutine != null)
            {
                StopCoroutine(_statusCheckRoutine);
                _statusCheckRoutine = null;
            }
        }

        /// <summary>
        /// Starts the status check routine after given delay.
        /// </summary>
        /// <param name="delay">The delay until the internet status starts being checked.</param>
        private void StartStatusCheckRoutine(float delay = 0f)
        {
            if (delay != 0f)
                StartCoroutine(DelayedCallback(delay, Callback));
            else
                Callback();

            void Callback() => _statusCheckRoutine = StartCoroutine(CheckForStatus());
        }

        /// <summary>
        /// Fires the status update events after scene load.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The mode in which the scene was loaded.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _target.InvokeStatusEvents();
    }
}
