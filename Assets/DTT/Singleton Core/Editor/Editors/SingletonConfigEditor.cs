#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Draws the custom inspector for the <see cref="SingletonConfigSO"/>.
    /// </summary>
    [CustomEditor(typeof(SingletonConfigSO))]
    [DTTHeader("dtt.singletoncore")]
    public class SingletonConfigEditor : DTTInspector
    {
        /// <summary>
        /// Draws the configuration properties.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif