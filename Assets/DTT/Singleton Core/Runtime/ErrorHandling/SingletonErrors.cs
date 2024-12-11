using System.Collections.Generic;

namespace DTT.Singletons
{
    /// <summary>
    /// Container of all <see cref="FlowError"/> values with corresponding messages.
    /// </summary>
    public static class SingletonErrors
    {
        #region Variables
        #region Private
        /// <summary>
        /// Holds all <see cref="FlowError"/> values with corresponding messages.
        /// </summary>
        private static readonly Dictionary<SingletonError, string> _errors = new Dictionary<SingletonError, string>()
        {
            {
                SingletonError.SINGLETONS_WITHOUT_PROFILE_ON_BUILD,
                "You have a singleton profile but do not have any singletons in it to be " +
                "used for bootstrapping.\n If this is intended, set the RunPreBuildCheck " +
                "flag to false. If not, make sure you to enable your singletons for " +
                "bootstrap inside the singleton profile window."
            },
            {
                SingletonError.PROFILE_WITHOUT_SINGLETONS_ON_BUILD,
                "You have singleton prefabs in the project but do not have a singleton " +
                        "profile.\n You can enable you singletons for bootstrap inside the " +
                        "singleton profile window."
            },
            {
                SingletonError.SINGLETON_BOOT_INFO_NOT_FOUND,
                "The {0} singleton has been accessed but its boot info could not be found. Make sure " +
                "it is either enabled for bootstrap or set for a lazy bootstrap."
            },
            {
                SingletonError.SINGLETON_NOT_BOOTSTRAPPED,
                "The {0} singleton is part of the profile but has not been bootstrapped before the first " +
                "scene and is not set to lazy. Something went wrong during the bootstrap operation."
            },
            {
                SingletonError.INVALID_PROFILE_ON_LAZY_INSTANTIATION,
                "An invalid profile has been detected during the lazy instantiation of the {0} singleton. " +
                "Make sure there is a profile in the project and the {0} singleton is enabled."
            },
            {
                SingletonError.SINGLETON_PRE_EXISTED_LAZY_INSTANTATION,
                "{0} its lazy creation call was made while it already existed. " +
                "Something went wrong during the creation of the singleton prefab."
            },
            {
                SingletonError.RUNTIME_INSTANCE_ACCES_IN_EDITOR,
                "The runtime singleton {0} its instance is accessed during editor mode. " +
                "Make sure to call it only during runtime."
            },
            {
                SingletonError.SINGLETON_DUPLICATE_DESTROYED,
                "The {0} singleton instance is being destroyed because there is " +
                "already one alive. Make sure no duplicate singletons are in the scenes."
            },
            {
                SingletonError.EDITOR_INSTANCE_ACCES_DURING_RUNTIME,
                "The editor singleton {0} its instance is accessed during play mode. " +
                "Make sure to call it only during edit mode."
            },
            {
                SingletonError.NOT_LOADED_BOOTSTRAP_SCENE,
                "The {0} scene singleton could not be created lazily because the scene at path {1} " +
                "was not loaded. Make sure to use the instance if the scene is loaded."
            },
            {
                SingletonError.INSTANCE_ACCES_DURING_APP_QUIT,
                "The {0} singleton instance was accessed while it was being destroyed by the application quit event. " +
                "This is probably  because of an OnDestroy call to {0} during application quit. There is no guarantee " +
                "that {0} will  be alive during this time. Use the 'Exists' property or 'OnBeforeDestroy' event instead."
            }
        };
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Retrieve a <see cref="SingletonError"/> message with optional formatted arguments.
        /// </summary>
        /// <param name="exception">The error of which the message to retrieve.</param>
        /// <param name="args">The arguments that are embedded in the message.</param>
        /// <returns>The message corresponding to the given error.</returns>
        public static string Get(SingletonError error, params object[] args)
        {
            if (args.Length != 0)
                return string.Format(_errors[error], args);
            else
                return _errors[error];
        }
        #endregion
        #endregion
    }

}
