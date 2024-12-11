using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// The boot mode types a <see cref="SingletonBehaviour{T}"/> can have.
    /// </summary>
    public enum BootModeType
    {
        /// <summary>
        /// The singleton won't be created at all.
        /// </summary>
        [InspectorName("Disabled")]
        [Tooltip("The singleton won't be created at all.")]
        DISABLED = 0,

        /// <summary>
        /// The singleton will be created before the first scene of the application is loaded.
        /// </summary>
        [InspectorName("Default")]
        [Tooltip("The singleton will be created before the first scene of the application is loaded.")]
        DEFAULT = 1,

        /// <summary>
        /// The singleton will be created when its instance is first used.
        /// </summary>
        [InspectorName("Lazy")]
        [Tooltip("The singleton will be created when its instance is first used.")]
        LAZY = 2,

        /// <summary>
        /// The singleton will be created when a specified scene is loaded and destroyed when
        /// the this scene is unloaded.
        /// </summary>
        [InspectorName("Scene")]
        [Tooltip("The singleton will be created when a specified scene is loaded and destroyed when the this scene is unloaded.")]
        SCENE_BASED = 3,
    }
}