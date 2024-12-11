#if UNITY_EDITOR

using DTT.Singletons.Exceptions;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// The <see cref="LazyEditorSingletonBehaviour{T}"/> is a generic class that provides all the 
    /// tools for an editor singleton that forces lazy initialization. This script should not be attached 
    /// to a game object in a scene or prefab. It will create a game object for itself and add 
    /// this component to it.
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming a lazy editor singleton.
    /// </typeparam>
    public class LazyEditorSingletonBehaviour<T> : EditorSingletonBehaviour<T> where T : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// The editor instance of the singleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (EditorApplication.isPlaying)
                    throw new SingletonInstanceException(
                        SingletonErrors.Get(SingletonError.EDITOR_INSTANCE_ACCES_DURING_RUNTIME, typeof(T)));

                // A null instance is assigned the first instance in the scene first and a created instance second.
                return p_Instance ?? (p_Instance = FindInstanceInScene()) ?? (p_Instance = CreateInstanceFromScript());
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Creates the singleton instance using a newly created game object and
        /// adding a component of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The created singleton instance.</returns>
        private static T CreateInstanceFromScript() => new GameObject(typeof(T).Name).AddComponent<T>();
        #endregion
        #endregion
    }
}

#endif