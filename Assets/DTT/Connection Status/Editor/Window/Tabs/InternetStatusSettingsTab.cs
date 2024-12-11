#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Draws the settings of the <see cref="InternetStatusWorker"/>.
    /// </summary>
    internal class InternetStatusSettingsTab : IInternetStatusWindowTab
    {
        /// <summary>
        /// The settings title.
        /// </summary>
        public string Title => "Settings";

        /// <summary>
        /// The state of the window.
        /// </summary>
        private readonly InternetStatusWindowState _windowState;
        
        /// <summary>
        /// Initializes the tab with the window state.
        /// </summary>
        /// <param name="windowState">The serialized window state.</param>
        public InternetStatusSettingsTab(InternetStatusWindowState windowState) => _windowState = windowState;

        /// <summary>
        /// Draws the settings gui controls.
        /// </summary>
        /// <param name="position">The position of the window.</param>
        public void OnTabGUI(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            OnTargetInfoGUI();
            OnTestingInfoGUI();
            OnOptionsGUI();
            if(EditorGUI.EndChangeCheck())
                _windowState.ApplyChangesToTarget();
        }
        
        /// <summary>
        /// For now does nothing as it draws using the updated prefab target properties.
        /// </summary>
        public void Refresh() { }

        /// <summary>
        /// Draws the target info gui controls.
        /// </summary>
        private void OnTargetInfoGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("Target", EditorStyles.largeLabel);
                EditorGUILayout.PropertyField(_windowState.PrefabTargetProperties.name);
                EditorGUILayout.PropertyField(_windowState.PrefabTargetProperties.address);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the testing info gui controls.
        /// </summary>
        private void OnTestingInfoGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("Testing", EditorStyles.largeLabel);
                EditorGUILayout.PropertyField(_windowState.PrefabTargetProperties.usesStatusOverride);
                
                OnStatusOverrideGUI();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the target options gui controls.
        /// </summary>
        private void OnOptionsGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("Options", EditorStyles.largeLabel);
                EditorGUILayout.PropertyField(_windowState.PrefabTargetProperties.pingInterval);
                EditorGUILayout.PropertyField(_windowState.PrefabTargetProperties.maxReconnectDuration);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the status override gui using the EditorInternetStatus to prevent usage of the
        /// InternetStatus.UNKNOWN value.
        /// </summary>
        private void OnStatusOverrideGUI()
        {
            SerializedProperty statusOverride = _windowState.PrefabTargetProperties.statusOverride;
            EditorInternetStatus status = GetEditorInternetStatus(statusOverride);
            statusOverride.enumValueIndex = (int)(InternetStatus)EditorGUILayout.EnumPopup(nameof(statusOverride), status);
        }
        
        /// <summary>
        /// Returns the editor internet status for a status override property ensuring
        /// InternetStatus.UNKNOWN is not used.
        /// </summary>
        /// <param name="statusOverride">The status override property.</param>
        /// <returns>The editor internet status.</returns>
        private EditorInternetStatus GetEditorInternetStatus(SerializedProperty statusOverride)
        {
            const int VALID_INTERNET_STATUS_COUNT = 3;
            
            // Clamp the integer value of the enum between zero and three 
            // to ensure InternetStatus.UNKNOWN is not used.
            int value = Mathf.Clamp(statusOverride.enumValueIndex , 1, VALID_INTERNET_STATUS_COUNT);
            return (EditorInternetStatus) value;
        }
    }
}

#endif