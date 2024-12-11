#if UNITY_EDITOR

using System;
using DTT.PublishingTools;
using System.Collections.Generic;
using System.Linq;
using DTT.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Draws the settings and listeners of the <see cref="InternetStatusWorker"/>.
    /// </summary>
    [DTTHeader("dtt.connectionstatus")]
    [HelpURL("https://dtt-dev.atlassian.net/wiki/spaces/UCP/pages/1736802387/Connection+Status")]
    internal class InternetStatusWindow : DTTEditorWindow
    {
        /// <summary>
        /// The state of the window.
        /// </summary>
        [SerializeField]
        private InternetStatusWindowState _state;

        /// <summary>
        /// The minimum size of the window.
        /// </summary>
        private readonly Vector2 _minSize = new Vector2(300f, 300f);

        /// <summary>
        /// The tabs that are part of the window.
        /// </summary>
        private readonly Dictionary<int, IInternetStatusWindowTab> _tabs =
            new Dictionary<int, IInternetStatusWindowTab>();

        /// <summary>
        /// The titles used for the tabs.
        /// </summary>
        private string[] _tabTitles;

        /// <summary>
        /// The key of the currently open tab.
        /// </summary>
        private int _currentTab;

        /// <summary>
        /// The currents status for the currently selected target.
        /// </summary>
        private InternetStatus _currentStatus;

        /// <summary>
        /// The error handler used by the window.
        /// </summary>
        private InternetStatusWindowErrorHandler _errorHandler;

        /// <summary>
        /// The width used for the status label in the toolbar.
        /// </summary>
        private const float STATUS_LABEL_WIDTH = 125f;

        /// <summary>
        /// Opens the window.
        /// </summary>
        /// <returns>The window.</returns>
        [MenuItem(ConnectionStatusEditorConfig.INTERNET_STATUS_WINDOW_MENU_NAME, priority = 0)]
        internal static InternetStatusWindow Open() =>
            GetWindow<InternetStatusWindow>(ConnectionStatusEditorConfig.INTERNET_STATUS_WINDOW_NAME);

        /// <summary>
        /// Initializes the window state.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _errorHandler = new InternetStatusWindowErrorHandler();
            _errorHandler.CheckHandlerInstance();
            
            if (_errorHandler.Errored)
                return;
            
            minSize = _minSize;
            
            if (_state == null)
                _state = new InternetStatusWindowState();

            _state.SelectedTargetChanged += OnSelectedTargetChanged;

            _tabs.Add(0, new InternetStatusTargetsTab(_state));
            _tabs.Add(1, new InternetStatusSettingsTab(_state));

            _tabTitles = _tabs.Values.Select(tab => tab.Title).ToArray();
            
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Draws the status and the currently open tab.
        /// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();

            if (!_errorHandler.Errored)
            {
                OnHeaderGUI();
                OnTabGUI();
            }
            else
            {
                EditorGUILayout.HelpBox(_errorHandler.ErrorMessage, MessageType.Warning);
            }
        }

        /// <summary>
        /// Draws the header gui controls.
        /// </summary>
        private void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                string[] displayOptions = _state.GetTargetNames();
                _state.SelectedTargetIndex = EditorGUILayout.Popup(_state.SelectedTargetIndex, displayOptions,
                    EditorStyles.toolbarPopup);

                GUILayout.Label("Status:", GUILayout.Width(STATUS_LABEL_WIDTH));
                GUILayout.FlexibleSpace();
                
                _state.Styles.SetStatusColor(_currentStatus);
                GUILayout.Label(_currentStatus.ToString().FromAllCapsToReadableFormat(), _state.Styles.StatusLabel);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the tab gui controls.
        /// </summary>
        private void OnTabGUI()
        {
            int newCurrentTab = GUILayout.Toolbar(_currentTab, _tabTitles);
            if (newCurrentTab != _currentTab)
            {
                // If a new tab has been selected, refresh its content before drawing.
                _currentTab = newCurrentTab;
                _tabs[_currentTab].Refresh();
            }

            _tabs[_currentTab].OnTabGUI(position);
        }

        /// <summary>
        /// Updates the display based on the play mode state change.
        /// </summary>
        /// <param name="state">The state change.</param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            _errorHandler.CheckHandlerInstance();
            
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    _state.ResetSerializedProperties();
                    _currentStatus = InternetStatus.UNKNOWN;
                    break;
                
                case PlayModeStateChange.EnteredPlayMode:
                    _state.ResetSerializedProperties();

                    if (InternetStatusManager.Worker != null)
                    {
                        // Make sure to set the play mode status as we are not getting any updates yet. 
                        SetPlayModeStatus();
                        
                        // Subscribe to all targets to update the current status.
                        InternetStatusWorker instance = InternetStatusManager.Worker;
                        for (int i = 0; i < instance.TargetCount; i++)
                        {
                            InternetStatusTarget target = instance.GetTarget(i);
                            target.StatusUpdate += (newStatus) => OnInternetStatusUpdateEvent(target, newStatus);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when the internet status of a target has been updated. It will update
        /// the current status and repaint the window if it was the selected target in the window.
        /// </summary>
        /// <param name="target">The target of which the status has been updated.</param>
        /// <param name="newStatus">The new status of the target.</param>
        private void OnInternetStatusUpdateEvent(InternetStatusTarget target, InternetStatus newStatus)
        {
            string selectedTargetName = InternetStatusManager.GetTarget(_state.SelectedTargetIndex).Name;
            if (selectedTargetName == target.Name)
            {
                _currentStatus = newStatus;
                Repaint();
            }
        }

        /// <summary>
        /// Called when the selected target in the window has changed. It will either set the play mode
        /// status or the current status based on the prefab.
        /// </summary>
        private void OnSelectedTargetChanged()
        {
            if (EditorApplication.isPlaying && InternetStatusManager.Worker != null)
                SetPlayModeStatus();
            else
                _currentStatus = (InternetStatus)_state.PrefabTargetProperties.currentStatus.enumValueIndex;
        }

        /// <summary>
        /// Sets the current status based on the selected target index using
        /// the global internet status handler instance.
        /// </summary>
        private void SetPlayModeStatus()
        {
            int index = _state.SelectedTargetIndex;
            _currentStatus = InternetStatusManager.GetTarget(index).CurrentStatus;
        }
    }
}

#endif