namespace DTT.Singletons
{
    /// <summary>
    /// The singleton error types.
    /// </summary>
    public enum SingletonError
    {
        /// <summary>
        /// A <see cref="SingletonBehaviour{T}"/> instance is called while it wasn't created
        /// as part of the bootstrap.
        /// </summary>
        SINGLETON_BOOT_INFO_NOT_FOUND = 0,

        /// <summary>
        /// A <see cref="SingletonBehaviour{T}"/> is in the profile but not bootstrapped.
        /// </summary>
        SINGLETON_NOT_BOOTSTRAPPED = 1,

        /// <summary>
        /// An invalid profile was detected during a lazy instantiation of a <see cref="SingletonBehaviour{T}"/>.
        /// </summary>
        INVALID_PROFILE_ON_LAZY_INSTANTIATION = 2,

        /// <summary>
        /// A <see cref="SingletonBehaviour{T}"/> lazy creation call was made while it already existed.
        /// </summary>
        SINGLETON_PRE_EXISTED_LAZY_INSTANTATION = 3,

        /// <summary>
        /// A duplicate singleton needed to be destroyed at awake. 
        /// </summary>
        SINGLETON_DUPLICATE_DESTROYED = 4,

        /// <summary>
        /// There are singletons in the project but no profile to set them up for bootstrap.
        /// </summary>
        SINGLETONS_WITHOUT_PROFILE_ON_BUILD = 5,

        /// <summary>
        /// A runtime singleton instance is accessed during editor mode.
        /// </summary>
        RUNTIME_INSTANCE_ACCES_IN_EDITOR = 6,

        /// <summary>
        /// There are no singletons in the project but there is a profile.
        /// </summary>
        PROFILE_WITHOUT_SINGLETONS_ON_BUILD = 7,

        /// <summary>
        /// An editor singleton instance is accessed during play mode.
        /// </summary>
        EDITOR_INSTANCE_ACCES_DURING_RUNTIME = 8,

        /// <summary>
        /// A singleton couldn't be created for a scene because the scene
        /// wasn't loaded during lazy creation.
        /// </summary>
        NOT_LOADED_BOOTSTRAP_SCENE = 9,

        /// <summary>
        /// A singleton instance was accessed while it was being destroyed by 
        /// an application quit event.
        /// </summary>
        INSTANCE_ACCES_DURING_APP_QUIT = 10,
    }
}

