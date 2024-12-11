using System;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// The SingletonBehaviourBase is an abstract class that defines 
    /// the shared attributes of the singleton behaviour implementation.
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming a singleton.</typeparam>
    public abstract class SingletonBehaviourBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Returns true if the singleton exists.
        /// </summary>
        public static bool Exists => p_Instance != null && !p_isBeingDestroyedByApp;

        /// <summary>
        /// Called before this singleton is being destroyed.
        /// </summary>
        public event Action<T> OnBeforeDestroy;
        #endregion
        #region Protected
        /// <summary>
        /// The protected instance property to be used by concrete implementations.
        /// </summary>
        protected static T p_Instance { get; set; }

        /// <summary>
        /// Whether this singleton is being destroyed by an application quit event.
        /// </summary>
        protected static bool p_isBeingDestroyedByApp { get; private set; }
        #endregion
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Ensures the state of the singleton is reset.
        /// </summary>
        protected virtual void Awake() => p_isBeingDestroyedByApp = false;
        #endregion
        #region Public
        /// <summary>
        /// Destroys this Singleton object object.
        /// </summary>
        public virtual void Destroy()
        {
            // No data is reset here. this is done in OnDestroy.
            Destroy(gameObject);
        }
        #endregion
        #region Protected
        /// <summary>
        /// Cleans up the singleton instance when the singleton is destroyed.
        /// Make sure this is called when overriding this method.
        /// </summary>
        protected virtual void OnDestroy()
        {
            OnBeforeDestroy?.Invoke(this as T);

            if (Exists && p_Instance == this)
                p_Instance = null;
        }
        #endregion
        #region Private
        /// <summary>
        /// Sets that the singleton is being destroyed by the application quit event.
        /// </summary>
        private void OnApplicationQuit() => p_isBeingDestroyedByApp = true;
        #endregion
        #endregion
    }

}

