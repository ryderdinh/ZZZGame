using DTT.Singletons.Exceptions;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTT.Singletons
{
    /// <summary>
    /// Provides bootstrap funtionalities for singleton behaviours.
    /// </summary>
    public static class SingletonBootstrapService
    {
        #region Variables
        #region Private
        /// <summary>
        /// The first scene that was loaded by the application.
        /// </summary>
        public static Scene FirstSceneLoaded { get; private set; }

        /// <summary>
        /// The profile containing the singletons to be bootstrapped.
        /// </summary>
        private static SingletonProfileSO _profile;

        /// <summary>
        /// The root transform used for bootstrapped singletons.
        /// </summary>
        private static Transform _behaviourRootTransform;

        /// <summary>
        /// The root transform used for lazy singletons.
        /// </summary>
        private static Transform _lazyBehaviourRootTransform;

        /// <summary>
        /// Whether the bootstrap function is currently executed.
        /// </summary>
        private static bool _booting;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Resets the service state.
        /// </summary>
        static SingletonBootstrapService()
        {
            _profile = null;
            _behaviourRootTransform = null;
            _booting = false;

            RefreshSceneManagementListeners();
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Returns whether a singletons existence has been set by the profile. This value
        /// is updated just before singleton creation and can thus be useful to stop recursion
        /// from happening when a component on the singleton prefab calls the singleton component
        /// before it has assigned its Instance value.
        /// </summary>
        /// <typeparam name="T">The type of singleton behaviour to check.</typeparam>
        /// <returns>Whether the singletons existence has been set by the profile.</returns>
        public static bool CheckSingletonExistence<T>() where T : MonoBehaviour
        {
            SingletonBootInfo info = _profile.GetBootInfo(typeof(T));
            if (info != null)
                return info.Exists;
            else
                return false;
        }

        /// <summary>
        /// Returns whether the type of singleton is part of the profile and thus enabled for bootstrap.
        /// <para>A singleton is part of the profile if it isn't disabled.</para>
        /// </summary>
        /// <typeparam name="T">The type of singleton to check on.</typeparam>
        /// <returns>Whether the type of singleton is enabled for bootstrap.</returns>
        public static bool IsEnabledForBootstrap<T>() where T : SingletonBehaviourBase<T> => _profile.GetBootInfo(typeof(T)) != null;

        /// <summary>
        /// Subscribes an action to be called before this singleton is being destroyed. It is recommended
        /// to use this instead of calling upon the instance in 'OnDestroy' or 'OnDisable'.
        /// </summary>
        /// <typeparam name="T">The type of singleton.</typeparam>
        /// <param name="action">The action to call.</param>
        public static void OnBeforeDestroy<T>(Action<T> action) where T : SingletonBehaviourBase<T>
        {
            SingletonBootInfo info = _profile.GetBootInfo(typeof(T));
            if (info != null)
                info.SceneObject.GetComponent<T>().OnBeforeDestroy += action;
        }
        #endregion
        #region Internal
        /// <summary>
        /// Instantiates a singleton using the singleton profile. This should only be used for
        /// ensuring singletons created before scene load can use other singleton instances in
        /// their initialization and lazily instantiatable singletons.
        /// </summary>
        /// <typeparam name="T">The type of singleton to instantiate.</typeparam>
        /// <returns>The created singleton component instance.</returns>
        internal static T InstantiateLazyFromProfile<T>() where T : MonoBehaviour
        {
            if (!CanBootstrapWithProfile())
                throw new SingletonBootFailException(
                    SingletonErrors.Get(SingletonError.INVALID_PROFILE_ON_LAZY_INSTANTIATION, typeof(T).Name));

            SingletonBootInfo info = _profile.GetBootInfo(typeof(T));
            if (info == null)
                throw new SingletonBootFailException(
                    SingletonErrors.Get(SingletonError.SINGLETON_BOOT_INFO_NOT_FOUND, typeof(T).Name));

            if (info.Exists)
                throw new SingletonBootFailException(
                    SingletonErrors.Get(SingletonError.SINGLETON_PRE_EXISTED_LAZY_INSTANTATION, typeof(T).Name));

            if (!_booting && !info.HasVariableTimeOfCreation)
                throw new SingletonBootFailException(
                    SingletonErrors.Get(SingletonError.SINGLETON_NOT_BOOTSTRAPPED, typeof(T).Name));

            if (string.IsNullOrEmpty(info.BootScenePath))
            {
                // If the boot process is currently in process or the singleton is to be created lazily,
                // return a lazily created singleton instance.
                return CreateSingletonLazy<T>(info);
            }
            else
            {
                // If the singleton has a boot scene path, a scene object has called this singleton
                // before it could be created of the sceneLoaded event and it should be created lazily.
                return CreateSingletonForScene<T>(info);
            }
        }

        /// <summary>
        /// Returns the lazy singleton root object to which lazy singletons 
        /// can attach in the  game scene to reduce clutter.
        /// </summary>
        /// <returns>The lazy singleton root game object.</returns>
        internal static Transform GetLazyRoot() => _lazyBehaviourRootTransform ?? (_lazyBehaviourRootTransform = CreateLazyRoot());
        #endregion
        #region Private  
        /// <summary>
        /// Returns whether it is possible to bootstrap singletons using given profile.
        /// </summary>
        /// <returns>Whether it is possible to bootstrap singletons using the profile.</returns>
        private static bool CanBootstrapWithProfile() => _profile != null && _profile.Count != 0;

        /// <summary>
        /// Refreshes the listening for scene management updates.
        /// </summary>
        private static void RefreshSceneManagementListeners()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <summary>
        /// Creates singletons that have the loaded scene as their bootstrap scene.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="loadSceneMode">The load scene mode in which the scene was loaded.</param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!CanBootstrapWithProfile())
                return;

            foreach (SingletonBootInfo info in _profile)
                if (!info.Exists && info.BootScenePath == scene.path)
                    CreateSingleton(info);
        }

        /// <summary>
        /// Destroys singletons that had the unloaded scene as their bootstrap scene.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private static void OnSceneUnloaded(Scene scene)
        {
            if (!CanBootstrapWithProfile())
                return;

            foreach (SingletonBootInfo info in _profile)
            {
                if (info.Exists && info.BootScenePath == scene.path)
                {
                    GameObject.Destroy(info.SceneObject);
                    info.Reset();
                }
            }
        }

        /// <summary>
        /// Creates the lazy singleton root object to which lazy singletons 
        /// can attach in the  game scene to reduce clutter.
        /// </summary>
        /// <returns>The created lazy singleton root game object.</returns>
        private static Transform CreateLazyRoot()
        {
            GameObject rootGameObject = new GameObject(SingletonConfig.LAZY_ROOT_NAME);
            GameObject.DontDestroyOnLoad(rootGameObject);
            return rootGameObject.transform;
        }

        /// <summary>
        /// Creates the singletons stored in the singleton profile before scene load using
        /// the RuntimeInitializeOnLoadMethod attribute.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            _booting = true;

            // Initialize the first loaded scene.
            FirstSceneLoaded = SceneManager.GetActiveScene();

            // Load singleton profile.
            _profile = Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);

            // Nothing should be created if the profile doens't 
            // exist or doesn't contain any singletons.
            if (!CanBootstrapWithProfile())
                return;

            // Create the singleton root gameobject and make it persistent across scenes.
            GameObject rootGameObject = new GameObject(SingletonConfig.NONLAZY_ROOT_NAME);
            GameObject.DontDestroyOnLoad(rootGameObject);

            _behaviourRootTransform = rootGameObject.transform;

            // Boot singletons that are bootstrappable.
            foreach (SingletonBootInfo info in _profile)
                if (info.IsBootstrappable)
                    CreateSingleton(info);

            _booting = false;
        }

        /// <summary>
        /// Creates a singleton before scene load using the given boot info.
        /// </summary>
        /// <param name="info">The info to use for creating the singleton.</param>
        private static void CreateSingleton(SingletonBootInfo info)
        {
            // Create singleton parented to the root singleton game object if it doesn't yet exist
            // and it is not set to be lazily instantiated.
            GameObject gameObject = GameObject.Instantiate(info.Prefab, _behaviourRootTransform);
            gameObject.name = info.Prefab.name;

            info.SceneObject = gameObject;
            info.Exists = true;
        }

        /// <summary>
        /// Creates a singleton to be used in its specified bootstrap scene.
        /// </summary>
        /// <typeparam name="T">The type of singleton to create.</typeparam>
        /// <param name="info">The info to use for creating the singleton.</param>
        /// <returns>The singleton component instance.</returns>
        private static T CreateSingletonForScene<T>(SingletonBootInfo info) where T : MonoBehaviour
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).path == info.BootScenePath)
                    return CreateSingletonLazy<T>(info);

            throw new SingletonCreationException(
                SingletonErrors.Get(SingletonError.NOT_LOADED_BOOTSTRAP_SCENE, typeof(T), info.BootScenePath));
        }

        /// <summary>
        /// Creates a singleton lazily using the given boot info. This should only be used for
        /// ensuring singletons created before scene load can use other singleton instances in
        /// their initialization and lazily instantiatable singletons.
        /// </summary>
        /// <typeparam name="T">The type of singleton to create.</typeparam>
        /// <param name="info">The info to use for creating the singleton.</param>
        /// <returns>The singleton component instance.</returns>
        private static T CreateSingletonLazy<T>(SingletonBootInfo info) where T : MonoBehaviour
        {
            // Set exists flag before instantiation to avoid recursion in the 
            // case of a component on the same prefab calling the singleton
            // before it is created.
            info.Exists = true;

            // Create singleton parented to the root singleton game object.    
            GameObject gameObject = GameObject.Instantiate(info.Prefab, _behaviourRootTransform);
            gameObject.name = info.Prefab.name;

            info.SceneObject = gameObject;

            return gameObject.GetComponent<T>();
        }
        #endregion
        #endregion
    }
}

