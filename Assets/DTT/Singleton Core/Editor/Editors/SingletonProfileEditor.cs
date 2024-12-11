#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Hides the serialized prefabs the profile holds and instead shows a button redirecting the user to the 
    /// singleton profile window.
    /// </summary>
    [CustomEditor(typeof(SingletonProfileSO))]
    [DTTHeader("dtt.singletoncore")]
    public class SingletonProfileEditor : DTTInspector
    {
        #region Methods
        #region Public
        /// <summary>
        /// Draws a button used to open the singleton profile window, hiding the profiles prefab list.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Profile"))
                ProfileWindow.Open();
        }
        #endregion
        #endregion
    }
}

#endif