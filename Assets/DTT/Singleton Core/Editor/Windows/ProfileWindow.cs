#if UNITY_EDITOR

using DTT.PublishingTools;
using System;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Displays all singletons in the project using a tree view layout.
    /// </summary>
    [DTTHeader("dtt.singletoncore", "DTT Managed Singletons", "Documentation.pdf")]
    public class ProfileWindow : DTTEditorWindow
    {
        /// <summary>
        /// The styles used for displaying the singletons.
        /// </summary>
        internal static ProfileStyles Styles { get; private set; }

        /// <summary>
        /// The content used for displaying the singletons.
        /// </summary>
        internal static ProfileContent Content { get; private set; }

        /// <summary>
        /// The serialized tree view layout.
        /// </summary>
        [SerializeField]
        private ProfileTreeViewLayout _treeViewLayout;

        /// <summary>
        /// The singleton configuration reference.
        /// </summary>
        private SingletonConfigSO _config;

        /// <summary>
        /// Whether the window requires a refresh or not.
        /// </summary>
        [NonSerialized]
        private bool _requiresRefresh;
        
        /// <summary>
        /// Opens the profile window.
        /// </summary>
        [MenuItem("Tools/DTT/SingletonCore/Profile", priority = 0)]
        public static void Open() => GetWindow<ProfileWindow>("ProfileWindow");

        /// <summary>
        /// Initializes the window state.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            Styles = new ProfileStyles();
            Content = new ProfileContent();

            _config = SingletonAssetDatabase.Config;
            if (_config == null)
                return;

            if (_treeViewLayout == null)
                _treeViewLayout = new ProfileTreeViewLayout();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Ensures a repaint where the treeview is refreshed before it is drawn if the
        /// window is open during entering of edit mode and no playmode reload was done.
        /// </summary>
        /// <param name="stateChange">The play mode state change.</param>
        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (!_config.playModeReload && stateChange == PlayModeStateChange.EnteredEditMode)
            {
                _requiresRefresh = true;
                Repaint();
            }
        }

        /// <summary>
        /// Draws the tree view layout.
        /// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();
            
            if (_config == null)
            {
                EditorGUILayout.HelpBox("No configuration could be loaded.", MessageType.Error);
                return;
            }

            if (_requiresRefresh)
            {
                _treeViewLayout.RefreshTreeView();
                _requiresRefresh = false;
            }

            _treeViewLayout.OnGUI(position.size);
        }
    }
}

#endif

