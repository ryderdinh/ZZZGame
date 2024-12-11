using System;
using DTT.Utils.Extensions;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Manages internet status by providing access to targets which can be pinged.
    /// </summary>
    public static class InternetStatusManager
    {
        /// <summary>
        /// The default status target.
        /// </summary>
        public static InternetStatusTarget DefaultTarget => GetTarget(_worker.DefaultTargetIndex);

        /// <summary>
        /// The worker instance.
        /// </summary>
        internal static InternetStatusWorker Worker => _worker;
        
        /// <summary>
        /// The internet status worker instance used to retrieve target data.
        /// </summary>
        private static InternetStatusWorker _worker;

        /// <summary>
        /// Returns an internet status target based on name or address. Returns null if the
        /// target could not be found. 
        /// </summary>
        /// <param name="nameOrAddress">The name or address of the target.</param>
        /// <returns>The internet status target. Null if not found.</returns>
        public static InternetStatusTarget GetTarget(string nameOrAddress) => _worker.GetTarget(nameOrAddress);

        /// <summary>
        /// Returns a target at a given index. 
        /// </summary>
        /// <param name="index">The zero based index of the target in the stored list of targets.</param>
        /// <returns>The target.</returns>
        public static InternetStatusTarget GetTarget(int index)
        {
            if (!index.InRange(0, _worker.TargetCount - 1))
                throw new ArgumentOutOfRangeException(nameof(index));
            
            return _worker.GetTarget(index);
        }

        /// <summary>
        /// Hooks up a new target status retriever to a target.
        /// </summary>
        /// <param name="target">The target to create the retriever for.</param>
        /// <returns>The created status retriever.</returns>
        internal static InternetStatusRetriever HookRetrieverToTarget(InternetStatusTarget target)
        {
            InternetStatusRetriever retriever = _worker.gameObject.AddComponent<InternetStatusRetriever>();
            retriever.SetTarget(target);
            return retriever;
        }
        
        /// <summary>
        /// Creates the worker before the first scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateWorker()
        {
            // Load the worker prefab component from the resources folder.
            InternetStatusWorker prefabComponent = Resources.Load<InternetStatusWorker>(nameof(InternetStatusWorker));
            if (prefabComponent == null)
            {
                throw new InvalidOperationException("Failed loading internet status worker prefab. " +
                                                    "Make sure it is available in a resources folder.");
            }

            // Create the worker instance.
            _worker = GameObject.Instantiate(prefabComponent);
            _worker.name = nameof(InternetStatusWorker);

            // Initialize the workers targets.
            for (int i = 0; i < _worker.TargetCount; i++)
            {
                InternetStatusTarget target = _worker.GetTarget(i);
                target.Reset();
                target.Initialize();
            }
            
            // Make sure the worker persists across scene loads.
            GameObject.DontDestroyOnLoad(_worker.gameObject);
        }
    }
}
