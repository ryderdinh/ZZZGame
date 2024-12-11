using DTT.Singletons.Exceptions;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// The <see cref="LazySingletonBehaviour{T}"/> is a generic class that provides all the 
    /// tools for a singleton but forces lazy initialization. This script should not be attached 
    /// to a game object in a scene or prefab. It will create a game object for itself and add 
    /// this component to it.
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming a lazy singleton.
    /// </typeparam>
    [HelpURL("https://dtt-dev.atlassian.net/wiki/spaces/UCP/pages/1381007405/SingletonCore")]
    public class LazySingletonBehaviour<T> : SingletonBehaviourBase<T> where T : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// The Lazy Singleton instance.
        /// </summary>
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

                return p_Instance ?? (p_Instance = CreateSingleton());
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Creates the Singleton game object, makes it persistent 
        /// across scenes and returns the component instance.
        /// </summary>
        /// <returns>The created singleton component instance.</returns>
        private static T CreateSingleton()
        {
            // Create the lazy singleton and parent it to the root.
            GameObject gameObject = new GameObject(typeof(T).Name);
            gameObject.transform.SetParent(SingletonBootstrapService.GetLazyRoot());

            // Add the singleton component and return the instance.
            return gameObject.AddComponent<T>();
        }
        #endregion
        #endregion
    }
}

