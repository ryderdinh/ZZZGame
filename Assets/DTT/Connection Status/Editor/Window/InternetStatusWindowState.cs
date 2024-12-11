#if UNITY_EDITOR

using System;
using DTT.Utils.Extensions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Represents the serialized state of the internet status window.
    /// </summary>
    [Serializable]
    internal class InternetStatusWindowState
    {
        /// <summary>
        /// The serialized tree view state.
        /// </summary>
        [SerializeField]
        private TreeViewState _treeViewState = new TreeViewState();
        
        /// <summary>
        /// The index of the currently selected target in the window.
        /// </summary>
        [SerializeField]
        private int _selectedTargetIndex = 0;

        /// <summary>
        /// The serialized properties of the internet status handler in the scene.
        /// </summary>
        private InternetStatusWorkerProperties _sceneWorkerProperties;

        /// <summary>
        /// The serialized properties of the selected internet status target in the scene.
        /// </summary>
        private InternetStatusTargetProperties _sceneTargetProperties;
        
        /// <summary>
        /// The serialized properties of the internet status handler in the project.
        /// </summary>
        private InternetStatusWorkerProperties _prefabWorkerProperties;

        /// <summary>
        /// The serialized properties of the selected internet status target in the project.
        /// </summary>
        private InternetStatusTargetProperties _prefabTargetProperties;

        /// <summary>
        /// The lazy styles used to draw the window.
        /// </summary>
        private Lazy<InternetStatusWindowStyles> _styles 
            = new Lazy<InternetStatusWindowStyles>(CreateStyles);

        /// <summary>
        /// The serialized tree view state.
        /// </summary>
        public TreeViewState TreeState => _treeViewState;

        /// <summary>
        /// Fired when the selected target in the window has changed.
        /// </summary>
        public event Action SelectedTargetChanged;

        /// <summary>
        /// The index of the currently selected target in the window.
        /// </summary>
        public int SelectedTargetIndex
        {
            get => _selectedTargetIndex;
            set
            {
                int oldValue = _selectedTargetIndex;
                _selectedTargetIndex = value;
                
                if(oldValue != _selectedTargetIndex)
                {
                    // Reset target properties if the selected target has changed.
                    _prefabTargetProperties = null;
                    _sceneTargetProperties = null;

                    // Let window know the selected target has changed so it can update the status.
                    SelectedTargetChanged.Invoke();
                }
            }
        }

        /// <summary>
        /// The serialized properties of the internet status handler in the project. 
        /// </summary>
        public InternetStatusWorkerProperties PrefabWorkerProperties
        {
            get
            {
                if (_prefabWorkerProperties == null)
                {
                    InternetStatusWorker worker = InternetStatusEditorUtility.GetWorker();
                    SerializedObject serializedObject = new SerializedObject(worker);
                    _prefabWorkerProperties = new InternetStatusWorkerProperties(serializedObject);
                }

                return _prefabWorkerProperties;
            }
        }

        /// <summary>
        /// The serialized properties of the selected internet status target in the project.
        /// </summary>
        public InternetStatusTargetProperties PrefabTargetProperties
        {
            get
            {
                if (_prefabTargetProperties == null)
                {
                    SerializedProperty target = _prefabWorkerProperties.targets.GetArrayElementAtIndex(_selectedTargetIndex);
                    _prefabTargetProperties = new InternetStatusTargetProperties(target);
                }

                return _prefabTargetProperties;
            }
        }

        /// <summary>
        /// The serialized properties of the selected internet status target in the scene.
        /// </summary>
        public InternetStatusTargetProperties SceneTargetProperties
        {
            get
            {
                if(_sceneTargetProperties == null)
                {
                    SerializedProperty target = _sceneWorkerProperties.targets.GetArrayElementAtIndex(_selectedTargetIndex);
                    _sceneTargetProperties = new InternetStatusTargetProperties(target);
                }

                return _sceneTargetProperties;
            }
        }

        /// <summary>
        /// The styles used to draw the window.
        /// </summary>
        public InternetStatusWindowStyles Styles => _styles.Value;

        /// <summary>
        /// Applies property changes to the serialized objects.
        /// </summary>
        public void ApplyChangesToTarget()
        {
            PrefabTargetProperties.ApplyChangesToObject();
            
            // If we are in playmode and the worker exists, we need to copy updated values over to it.
            if (EditorApplication.isPlaying && InternetStatusManager.Worker != null)
            {
                if (_sceneWorkerProperties == null)
                {
                    SerializedObject serializedObject = new SerializedObject(InternetStatusManager.Worker);
                    _sceneWorkerProperties = new InternetStatusWorkerProperties(serializedObject);
                }
                
                PrefabTargetProperties.CopyTo(SceneTargetProperties);
            }
        }

        /// <summary>
        /// Resets serialized property values stored by the state.
        /// </summary>
        public void ResetSerializedProperties()
        {
            _prefabWorkerProperties = null;
            _prefabTargetProperties = null;
            _sceneWorkerProperties = null;
            _sceneTargetProperties = null;
        }
        
        /// <summary>
        /// Ensures the bounds of the default target index are in line with the target list size.
        /// </summary>
        public void EnsureCorrectDefaultTargetBounds()
        {
            int defaultTargetIndex = _prefabWorkerProperties.defaultTargetIndex.intValue;
            int maxIndexValue = _prefabWorkerProperties.targets.arraySize - 1;
            if (!defaultTargetIndex.InRange(0, maxIndexValue))
            {
                _prefabWorkerProperties.defaultTargetIndex.intValue = maxIndexValue;
                _prefabWorkerProperties.ApplyChanges();
            }
        }

        /// <summary>
        /// Returns the names of targets stored by the handler.
        /// </summary>
        /// <returns>The names of the targets.</returns>
        public string[] GetTargetNames()
        {
            SerializedProperty targets = PrefabWorkerProperties.targets;
            string[] names = new string[targets.arraySize];
            for (int i = 0; i < names.Length; i++)
                names[i] = targets.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;

            return names;

        }

        /// <summary>
        /// Creates the styles used for drawing the window.
        /// </summary>
        /// <returns>The styles.</returns>
        private static InternetStatusWindowStyles CreateStyles() => new InternetStatusWindowStyles();
    }
}

#endif
