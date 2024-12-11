#if UNITY_EDITOR

using System;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Represents a singleton that is created from script using lazy instantation.
    /// </summary>
    public struct SingletonScript
    {
        #region Variables
        #region Public
        /// <summary>
        /// The script text asset.
        /// </summary>
        public readonly TextAsset asset;

        /// <summary>
        /// The type of the singleton.
        /// </summary>
        public readonly Type singletonType;

        /// <summary>
        /// Whether the singleton prefab is valid or not.
        /// </summary>
        public bool IsValid => asset != null && singletonType != null;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the singleton script.
        /// </summary>
        /// <param name="singletonType">The type of the singleton.</param>
        /// <param name="asset">The script text asset.</param>
        public SingletonScript(Type singletonType, TextAsset asset)
        {
            this.singletonType = singletonType;
            this.asset = asset;
        }
        #endregion
    }
}

#endif
