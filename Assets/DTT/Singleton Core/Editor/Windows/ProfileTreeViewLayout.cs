#if UNITY_EDITOR

using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// The serializable layout used to draw the tree view and its dependencies.
    /// </summary>
    [Serializable]
    public class ProfileTreeViewLayout
    {
        /// <summary>
        /// The serialized tree view state.
        /// </summary>
        [SerializeField]
        private TreeViewState _state;

        /// <summary>
        /// The serialized singleton core configuration.
        /// </summary>
        [SerializeField]
        private SingletonConfigSO _config;

        /// <summary>
        /// The serialized header state.
        /// </summary>
        [SerializeField]
        private MultiColumnHeaderState _headerState;

        /// <summary>
        /// The vertical margin used for some layout items.
        /// </summary>
        private const float VERTICAL_MARGIN = 5f;

        /// <summary>
        /// The profile tree view.
        /// </summary>
        private ProfileTreeView _treeView;

        /// <summary>
        /// The profile tree view its search field.
        /// </summary>
        private SearchField _searchField;

        /// <summary>
        /// The height of the toolbar.
        /// </summary>
        private const float TOOLBAR_HEIGHT = 20f;

        /// <summary>
        /// Draws the tree view layout using a window size it requires.
        /// </summary>
        /// <param name="windowSize">The window size that is used to constrain the layout.</param>
        public void OnGUI(Vector2 windowSize)
        {
            ProfileTreeView treeView = GetTreeViewLazy();

            if (_treeView.CouldInitialize)
            {
                treeView.ValidateContent();
                DrawTreeViewToolbar(treeView, windowSize);
                treeView.OnGUI(GetTreeViewRect(windowSize));
            }
            else
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode && !_config.playModeReload)
                {
                    EditorGUILayout.HelpBox("The singleton profile is not available during playmode.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("No singletons found in the project. Add a prefab " +
                    "containing a valid singleton component to the project to make use of this " +
                    "window. Then press the Refresh button.", MessageType.Info);

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Refresh", DTTGUI.styles.Button))
                        RefreshTreeView();
                }
            }
        }

        /// <summary>
        /// Refreshes the asset database and reloads the tree view to update its state.
        /// </summary>
        internal void RefreshTreeView()
        {
            AssetDatabase.Refresh();
            GetTreeViewLazy().Reload();
        }

        /// <summary>
        /// Returns the tree view references and also lazily creates the necessary dependencies
        /// <para>
        /// Unity won't allow these initialization operations to happen during serialization
        /// which is why this is not done inside the constructor.
        /// </para>
        /// </summary>
        /// <returns>The profile tree view reference.</returns>
        internal ProfileTreeView GetTreeViewLazy()
        {
            if (_state == null)
                _state = new TreeViewState();

            if (_config == null)
                _config = SingletonAssetDatabase.Config;

            if (_treeView == null)
            {
                _treeView = new ProfileTreeView(_state, GetInitializedHeader());
                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            }

            return _treeView;
        }

        /// <summary>
        /// Draws the toolbar for the tree view.
        /// </summary>
        /// <param name="treeView">The tree view reference.</param>
        /// <param name="windowSize">The size of the window.</param>
        private void DrawTreeViewToolbar(ProfileTreeView treeView, Vector2 windowSize)
        {
            GUIDrawTools.Separator(EditorGUIUtility.isProSkin ? DTTColors.dark.line : DTTColors.light.line);

            Rect rect = GetToolbarRect(windowSize);
            GUI.BeginGroup(rect, EditorStyles.toolbar);
            Rect extraOptions = DrawExtraOptionsButton();
            Rect refresh = DrawRefreshButton(extraOptions);
            DrawSearchField(refresh, rect, treeView);
            GUI.EndGroup();
        }

        /// <summary>
        /// Draws a button that opens the configuration asset.
        /// </summary>
        /// <param name="last"The last </param>
        /// <returns></returns>
        private Rect DrawExtraOptionsButton()
        {
            Rect rect = new Rect(0, 0, TOOLBAR_HEIGHT, TOOLBAR_HEIGHT);

            if (GUI.Button(rect, ProfileWindow.Content.ExtraOptionsButton, ProfileWindow.Styles.ToolbarButton))
            {
                Type inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
                EditorWindow window = EditorWindow.GetWindow(inspectorType);
                window.Focus();

                AssetDatabase.OpenAsset(_config);
            }

            return rect;
        }

        /// <summary>
        /// Draws the refresh button using the last drawn control's rectangle 
        /// and returns its own.
        /// </summary>
        /// <param name="last">The rectangle of the last drawn control.</param>
        /// <returns>The rectangle the button is drawn inside.</returns>
        private Rect DrawRefreshButton(Rect last)
        {
            Rect rect = new Rect(last.x + last.width, last.y, TOOLBAR_HEIGHT, TOOLBAR_HEIGHT);
            if (GUI.Button(rect, ProfileWindow.Content.RefreshIcon, ProfileWindow.Styles.ToolbarButton))
                RefreshTreeView();

            return rect;
        }

        /// <summary>
        /// Draws the search field used by the tree view.
        /// </summary>
        /// <param name="last">The rectangle of the last drawn control.</param>
        /// <param name="toolbarRect">The whole toolbar rectangle.</param>
        /// <param name="treeView">The tree view reference.</param>
        /// <returns>The rectangle the searchfield is drawn inside.</returns>
        private Rect DrawSearchField(Rect last, Rect toolbarRect, ProfileTreeView treeView)
        {
            float width = toolbarRect.width - TOOLBAR_HEIGHT * 2f - VERTICAL_MARGIN;
            Rect rect = new Rect(last.x + last.width + 4f, last.y + 2f, width, toolbarRect.height);
            treeView.searchString = _searchField.OnToolbarGUI(rect, treeView.searchString);
            return rect;
        }

        /// <summary>
        /// Returns the rectangle for the tree view.
        /// </summary>
        /// <param name="windowSize">The size of the window.</param>
        /// <returns>The rectangle for the tree view.</returns>
        private Rect GetTreeViewRect(Vector2 windowSize)
        {
            float controlHeight = windowSize.y - DTTHeaderGUI.HEADER_HEIGHT - VERTICAL_MARGIN * 2f - 12f;
            return GUILayoutUtility.GetRect(windowSize.x, controlHeight);
        }

        /// <summary>
        /// Returns the rectangle for the toolbar.
        /// </summary>
        /// <param name="windowSize">The size of the window.</param>
        /// <returns>The rectangle for the toolbar.</returns>
        private Rect GetToolbarRect(Vector2 windowSize) => GUILayoutUtility.GetRect(windowSize.x, 20f);

        /// <summary>
        /// Returns the intialized header resizing it if it was serialzed.
        /// </summary>
        /// <returns>The initialized header.</returns>
        private MultiColumnHeader GetInitializedHeader()
        {
            bool serialized = _headerState != null;

            MultiColumnHeaderState defaultState = GetDefaultHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(_headerState, defaultState))
                MultiColumnHeaderState.OverwriteSerializedFields(_headerState, defaultState);

            _headerState = defaultState;

            MultiColumnHeader header = new MultiColumnHeader(_headerState);
            if (!serialized)
                header.ResizeToFit();

            return header;
        }

        /// <summary>
        /// Returns the default header state.
        /// </summary>
        /// <returns>The default header state.</returns>
        private MultiColumnHeaderState GetDefaultHeaderState()
        {
            return new MultiColumnHeaderState(new ProfileTreeViewCollumn[]
            {
                new TagColumn(this),
                new TypeCollumn(this),
                new NameCollumn(this),
                new BootModeCollumn(this),
                new BootDataCollumn(this),
                new EditCollumn(this)
            });
        }
    }
}


#endif