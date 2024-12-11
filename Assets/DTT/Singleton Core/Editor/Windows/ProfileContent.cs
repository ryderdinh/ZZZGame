#if UNITY_EDITOR

using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// The content used for drawing the singleton profile window.
    /// </summary>
    public class ProfileContent : GUIContentCache
    {
        #region Variables
        #region Public
        /// <summary>
        /// The content for a refresh icon.
        /// </summary>
        public GUIContent RefreshIcon => base[nameof(RefreshIcon)];

        /// <summary>
        /// The content for a runtime tag.
        /// </summary>
        public GUIContent RuntimeTag => base[nameof(RuntimeTag)];

        /// <summary>
        /// The content for an editor tag.
        /// </summary>
        public GUIContent EditorTag => base[nameof(EditorTag)];

        /// <summary>
        /// The content for a script tag.
        /// </summary>
        public GUIContent ScriptTag => base[nameof(ScriptTag)];

        /// <summary>
        /// The content for a prefab tag.
        /// </summary>
        public GUIContent PrefabTag => base[nameof(PrefabTag)];

        /// <summary>
        /// The content for an extra options button.
        /// </summary>
        public GUIContent ExtraOptionsButton => base[nameof(ExtraOptionsButton)];
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the content.
        /// </summary>
        public ProfileContent()
        {
            Add(nameof(RefreshIcon), () =>
            {
                GUIContent content = new GUIContent(EditorGUIUtility.IconContent("Refresh"));
                content.tooltip = "Refresh the window to include newly created prefabs.";
                return content;
            });

            Add(nameof(RuntimeTag), () =>
            {
                GUIContent content = new GUIContent("Runtime");
                content.tooltip = "This is singleton will only be used during runtime.";
                return content;
            });

            Add(nameof(ExtraOptionsButton), () =>
            {
                Texture texture = EditorTextures.Load<Texture>(
                    SingletonEditorConfig.FULL_PACKAGE_NAME,
                    EditorGUIUtility.isProSkin ? "options_light.png" : "options_dark.png"
                );
                GUIContent content = new GUIContent(texture);
                content.tooltip = "Open the configuration asset.";
                return content;
            });

            Add(nameof(EditorTag), () =>
            {
                GUIContent content = new GUIContent("Editor");
                content.tooltip = "This singleton will only be used during edit mode.";
                return content;
            });

            Add(nameof(ScriptTag), () =>
            {
                GUIContent content = new GUIContent(EditorGUIUtility.FindTexture("cs Script Icon"));
                content.tooltip = "This singleton is script only";
                return content;
            });

            Add(nameof(PrefabTag), () =>
            {
                GUIContent content = new GUIContent(EditorGUIUtility.FindTexture("Prefab Icon"));
                content.tooltip = "This singleton is part of a prefab asset.";
                return content;
            });
        }
        #endregion
    }
}

#endif
