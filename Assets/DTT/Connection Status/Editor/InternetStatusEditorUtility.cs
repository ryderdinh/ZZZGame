#if UNITY_EDITOR

using System;
using DTT.Utils.EditorUtilities;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Holds the assets used for internet status management.
    /// </summary>
    [InitializeOnLoad]
    internal static class InternetStatusEditorUtility
    {
        /// <summary>
        /// The cached <see cref="InternetStatusWorker"/> prefab component reference.
        /// </summary>
        private static InternetStatusWorker _worker;
        
        /// <summary>
        /// Creates the <see cref="InternetStatusWorker"/> prefab if it doesn't exist yet.
        /// </summary>
        static InternetStatusEditorUtility()
        {
            // Create the handler prefab if it doesn't yet exist.
            if (!File.Exists(ConnectionStatusEditorConfig.HandlerPrefabLocation))
                CreateWorkerPrefab();
        }

        /// <summary>
        /// The <see cref="InternetStatusWorker"/> prefab component reference.
        /// </summary>
        public static InternetStatusWorker GetWorker()
        {
            if (_worker == null)
                _worker = AssetDatabaseUtility.GetComponentInPrefab<InternetStatusWorker>
                    (ConnectionStatusEditorConfig.HandlerPrefabLocation);

            return _worker;
        }
        
        /// <summary>
        /// Creates the <see cref="InternetStatusWorker"/> prefab.
        /// </summary>
        private static void CreateWorkerPrefab()
        {
            // Ensure the internet status project folders exist.
            Directory.CreateDirectory(ConnectionStatusEditorConfig.ProjectFolder);
            Directory.CreateDirectory(ConnectionStatusEditorConfig.ResourcesFolder);

            // Create the internet status handler game object.
            GameObject gameObject = new GameObject(ConnectionStatusEditorConfig.WORKER_PREFAB_NAME);
            gameObject.AddComponent<InternetStatusWorker>();

            // Create the prefab.
            AssetDatabaseUtility.CreatePrefabAtPath(ConnectionStatusEditorConfig.HandlerPrefabLocation, gameObject);
        }
    }
}

#endif