using System;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// Holds information regarding the bootstrap of a singleton.
    /// </summary>
    [Serializable]
    public class SingletonBootInfo
    {
        /// <summary>
        /// The serializable singleton type value.
        /// </summary>
        [Serializable]
        private class SingletonType
        {
            /// <summary>
            /// The actual system type value.
            /// </summary>
            public Type Value
            {
                get
                {
                    if (_value != null)
                    {
                        // Return the value if it is initialized.
                        return _value;
                    }

                    try
                    {
                        // Try returning the assigned value using the static GetType method.
                        return _value = Type.GetType(_assemblyQualifiedName, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(
                            $"The assembly qualified name {_assemblyQualifiedName} is invalid. Resetting it.\n" +
                            e.Message);
                        _assemblyQualifiedName = string.Empty;
                        return null;
                    }
                }
                set
                {
                    _value = value;
                    _assemblyQualifiedName = value != null ? value.AssemblyQualifiedName : string.Empty;
                }
            }

            /// <summary>
            /// The assembly qualified name for this type.
            /// </summary>
            [SerializeField]
            private string _assemblyQualifiedName;

            /// <summary>
            /// The actual system type value.
            /// </summary>
            private Type _value;

            /// <summary>
            /// Creates a new serializable type without a value.
            /// </summary>
            public SingletonType() => _assemblyQualifiedName = string.Empty;

            /// <summary>
            /// Creates a new singleton type based on given Type value.
            /// </summary>
            /// <param name="type">The type to serialize.</param>
            public SingletonType(Type type) =>
                _assemblyQualifiedName = type != null ? type.AssemblyQualifiedName : string.Empty;
        }

        /// <summary>
        /// The prefab used for the bootstrap.
        /// </summary>
        [SerializeField]
        private GameObject _prefab;

        /// <summary>
        /// The singleton game object in the scene created using the prefab.
        /// </summary>
        [SerializeField]
        private SingletonType _singletonType;

        /// <summary>
        /// The path towards the scene in which the singleton will exist.
        /// </summary>
        [SerializeField]
        private string _bootScenePath;

        /// <summary>
        /// The boot mode for the singleton.
        /// </summary>
        [SerializeField]
        private BootModeType _bootMode;

        /// <summary>
        /// The prefab used for the bootstrap.
        /// </summary>
        public GameObject Prefab => _prefab;

        /// <summary>
        /// The singleton game object in the scene created using the prefab.
        /// </summary>
        public GameObject SceneObject
        {
            get => _sceneObject;
            internal set => _sceneObject = value;
        }

        /// <summary>
        /// The singleton type.
        /// </summary>
        public Type SingletonTypeValue => _singletonType.Value;

        /// <summary>
        /// The path towards the scene in which the singleton will exist.
        /// <para>Has a value if the singleton is constrained to one scene. Is null otherwise.</para>
        /// </summary>
        public string BootScenePath
        {
            get => _bootScenePath;
            internal set => _bootScenePath = value;
        }

        /// <summary>
        /// The boot mode for the singleton.
        /// </summary>
        public BootModeType BootMode
        {
            get => _bootMode;
            internal set => _bootMode = value;
        }

        /// <summary>
        /// Whether the singleton exists.
        /// </summary>
        public bool Exists
        {
            get => _exists;
            internal set => _exists = value;
        }

        /// <summary>
        /// Whether the singleton can be bootstrapped before the first scene.
        /// </summary>
        public bool IsBootstrappable
        {
            get
            {
                bool isDefault = !_exists && _bootMode != BootModeType.LAZY && string.IsNullOrEmpty(_bootScenePath);
                bool isSceneBasedAtFirstScene = !string.IsNullOrEmpty(_bootScenePath) && _bootScenePath == SingletonBootstrapService.FirstSceneLoaded.path;
                return isDefault || isSceneBasedAtFirstScene;
            }
        }

        /// <summary>
        /// Whether the singleton can be created at variable time during the application
        /// its lifecycle. This could be because it is lazy or has a boot scene path.
        /// </summary>
        public bool HasVariableTimeOfCreation => _bootMode == BootModeType.LAZY || !string.IsNullOrEmpty(_bootScenePath);

        /// <summary>
        /// Whether the singleton boot info is valid or not.
        /// </summary>
        public bool IsValid => _prefab != null && _singletonType.Value != null && SingletonValidator.IsSubClassOfSingleton(_singletonType.Value);

        /// <summary>
        /// Whether the singleton exists.
        /// </summary>
        [NonSerialized]
        private bool _exists;

        /// <summary>
        /// The singleton game object in the scene created using the prefab.
        /// </summary>
        [NonSerialized]
        private GameObject _sceneObject;

        #region Constructors
        /// <summary>
        /// Sets the initial state of the singleton boot info.
        /// </summary>
        /// <param name="prefab">The prefab to use for the boostrap.</param>
        /// <param name="singletonType">The singleton type.</param>
        /// <param name="bootMode">The initial boot mode for the singleton.</param>
        public SingletonBootInfo(GameObject prefab, Type singletonType, BootModeType bootMode)
        {
            _prefab = prefab;
            _singletonType = new SingletonType(singletonType);
            _bootMode = bootMode;
        }
        #endregion

        /// <summary>
        /// Resets the boot info state.
        /// </summary>
        public void Reset()
        {
            _exists = false;
            _sceneObject = null;
        }
    }
}


