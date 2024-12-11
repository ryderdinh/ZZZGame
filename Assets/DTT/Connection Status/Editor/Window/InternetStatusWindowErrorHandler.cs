#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Manages errors for the internet status window.
    /// </summary>
    internal class InternetStatusWindowErrorHandler
    {
        /// <summary>
        /// The types of errors that can occur in the window.
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// No error has occured.
            /// </summary>
            NONE = 0,
            
            /// <summary>
            /// The handler prefab was not found in the project.
            /// </summary>
            MISSING_PREFAB = 1,
            
            /// <summary>
            /// The handler singleton was not setup for bootstrap.
            /// </summary>
            NOT_CREATED_BY_MANAGER = 2,
        }

        /// <summary>
        /// The error types with their corresponding messages.
        /// </summary>
        private readonly Dictionary<ErrorType, string> _messages = new Dictionary<ErrorType, string>
        {
            {
                ErrorType.NONE,
                string.Empty
            },
            { 
                ErrorType.MISSING_PREFAB, 
                "The connection status prefab is missing in the project."
                
            },
            {
                ErrorType.NOT_CREATED_BY_MANAGER, 
                "The internet status worker is not created during bootstrap. " +
                "Make sure the prefab can be found in a resource folder"
            }
        };

        /// <summary>
        /// The current active error.
        /// </summary>
        private ErrorType _currentError;

        /// <summary>
        /// Whether an error has been detected.
        /// </summary>
        public bool Errored => _currentError != ErrorType.NONE;

        /// <summary>
        /// The current error message.
        /// </summary>
        public string ErrorMessage => _messages[_currentError];

        /// <summary>
        /// Checks whether the internet status handler is available.
        /// </summary>
        public void CheckHandlerInstance()
        {
            if (EditorApplication.isPlaying)
            {
                if (InternetStatusManager.Worker == null)
                    _currentError = ErrorType.NOT_CREATED_BY_MANAGER;
            }
            else
            {
                try
                {
                    InternetStatusEditorUtility.GetWorker();
                }
                catch
                {
                    _currentError = ErrorType.MISSING_PREFAB;
                }
            }
        }
    }
}

#endif
