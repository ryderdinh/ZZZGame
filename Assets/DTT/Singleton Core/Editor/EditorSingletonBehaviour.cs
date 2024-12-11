#if UNITY_EDITOR

using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Represents a singleton usable in edit mode. 
    /// </summary>
    /// <typeparam name="T">The type of the monobehaviour that is becoming an editor singleton.</typeparam>
    public abstract class EditorSingletonBehaviour<T> : SingletonBehaviourBase<T> where T : MonoBehaviour
    {
        #region Methods
        #region Public
        /// <summary>
        /// Destroys this editor singleton.
        /// </summary>
        public override void Destroy()
        {
            if (p_Instance != null)
            {
                // Destroy the game object using 'DestroyImmediate' since this is the editor.
                DestroyImmediate(gameObject);

                // Reset the instance here since 'OnDestroy' won't be called.
                p_Instance = null;
            }
        }

        /// <summary>
        /// Returns an instance of the singleton found in the scene.
        /// <para>This is used internally to reassign the instance after an assembly reload.</para>
        /// </summary>
        /// <returns>The instance of the singleton found in the scene.</returns>
        public static T FindInstanceInScene() => p_Instance ?? FindObjectOfType<T>();
        #endregion
        #endregion
    }
}

#endif