using DTT.Singletons.Exceptions;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// The SingletonBehaviour is a generic abstract class that provides all the tools for a singleton
    /// that uses a non-lazy initialization. This component should be part of a prefab asset so it can
    /// be displayed and setup for bootstrap inside the singleton profile window. 
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming a singleton.</typeparam>
    [HelpURL("https://dtt-dev.atlassian.net/wiki/spaces/UCP/pages/1381007405/SingletonCore")]
    public abstract class SingletonBehaviour<T> : SingletonBehaviourBase<T> where T : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// The Singleton instance.
        /// </summary>
        /// <remarks>
        /// Will use the singleton bootstrap service to instantiate the instance lazily using
        /// the profile if the singleton instance hasn't been assigned yet but assesed anyway.
        /// </remarks>
        public static T Instance
        {
            get
            {
                if (!Application.isPlaying)
                    throw new SingletonInstanceException(
                        SingletonErrors.Get(SingletonError.RUNTIME_INSTANCE_ACCES_IN_EDITOR, typeof(T).Name));

                if (p_isBeingDestroyedByApp)
                    throw new SingletonInstanceException(
                        SingletonErrors.Get(SingletonError.INSTANCE_ACCES_DURING_APP_QUIT, typeof(T).Name));

                if (p_Instance == null)
                {
                    if (SingletonBootstrapService.CheckSingletonExistence<T>())
                    {
                        // If the existence of the singleton has been set by the 
                        // bootstrap service but the instance is null, a component
                        // on the singleton prefab is calling the instance before the
                        // singleton itself can set it. To avoid recursion from happening
                        // the instance is set using an expensive FindObjectOfType call.
                        p_Instance = FindObjectOfType<T>();
                    }
                    else
                    {
                        // If the existence of the singleton has not yet been set by the bootstrap
                        // service, we can return a lazy instantiation using the singleton profile.
                        p_Instance = SingletonBootstrapService.InstantiateLazyFromProfile<T>();
                    }
                }

                return p_Instance;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Initializes the singleton instance and handles the duplicate singleton conflict.
        /// </summary>
        protected override void Awake()
        {
            if (Exists && p_Instance != this)
            {
                Debug.LogWarning(SingletonErrors.Get(SingletonError.SINGLETON_DUPLICATE_DESTROYED, typeof(T).Name));

                // If a singleton is already active and it isn't this one, 
                // this is a duplicate so it needs to be destroyed.
                Destroy(gameObject);
            }
            else
            {
                // DontDestroyOnLoad is not called on this component's game
                // object because the singleton root already has this attribute.

                // Assign the instance this component's value.
                p_Instance = this as T;
            }
        }
        #endregion
        #endregion
    }

}
