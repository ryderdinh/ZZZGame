#if UNITY_EDITOR

using DTT.Singletons.Exceptions;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// The <see cref="LazyEditorPrefabSingletonBehaviour{T}"/> is a generic class that provides all the 
    /// tools for an editor singleton that is part of a prefab. This script should be attached 
    /// to a prefab. It will use the prefab to create the instance the first time.
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming a lazy editor singleton.
    /// </typeparam>
    public abstract class LazyEditorPrefabSingletonBehaviour<T> : EditorSingletonBehaviour<T> where T : MonoBehaviour
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
                return p_Instance ?? (p_Instance = FindInstanceInScene()) ?? (p_Instance = CreateInstanceFromPrefab());
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Creates the singleton instance using the prefab it is attached to in the project.
        /// </summary>
        /// <returns>The created singleton instance.</returns>
        private static T CreateInstanceFromPrefab()
        {
            GameObject[] prefabs = SingletonImporter.GetImportedPrefabs();

            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject prefab = prefabs[i];
                if (prefab.GetComponent<T>() != null)
                    return Instantiate(prefab).GetComponent<T>();
            }

            Debug.LogWarning("Tried creating editor singleton instance from prefab, but prefab wasn't found.");
            return null;
        }
        #endregion
        #endregion
    }
}

#endif