#if UNITY_EDITOR

using System;
using DTT.Utils.EditorUtilities;
using UnityEditor;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Stores the properties of the <see cref="InternetStatusWorker"/> component.
    /// </summary>
    internal class InternetStatusWorkerProperties : SerializedPropertyCache
    {
        /// <summary>
        /// Whether the worker has targets.
        /// </summary>
        public bool HasTargets => targets.arraySize != 0;

        /// <summary>
        /// The default target index property.
        /// </summary>
        public SerializedProperty defaultTargetIndex => base[nameof(defaultTargetIndex)];
        
        /// <summary>
        /// The targets property.
        /// </summary>
        public SerializedProperty targets => base[nameof(targets)];
        
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        /// <param name="serializedObject">The serialized object.</param>
        public InternetStatusWorkerProperties(SerializedObject serializedObject) : base(serializedObject) { }
    }
}

#endif