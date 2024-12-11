namespace DTT.Singletons
{
    /// <summary>
    /// Holds all configuration data related to the singleton core package. Make sure 
    /// to use the strings stored here for all singleton-core related operations.
    /// </summary>
    [System.Serializable]
    public class SingletonConfig
    {
        #region Variables    
        #region Public
        /// <summary>
        /// The name of the singleton profile asset. Use this when
        /// creating in- or loading the profile from resources.
        /// </summary>
        public const string PROFILE_ASSET_NAME = "SingletonProfile";

        /// <summary>
        /// The load path relative to the resources folder that
        /// should be used when loading the singleton profile.
        /// </summary>
        public const string PROFILE_LOAD_PATH = PROFILE_ASSET_NAME;

        /// <summary>
        /// The name of the root game object to which  
        /// all lazy singletons are attached.
        /// </summary>
        public const string LAZY_ROOT_NAME = "LazySingletons";

        /// <summary>
        /// The name of the root game object to which  
        /// all non-lazy singletons are attached.
        /// </summary>
        public const string NONLAZY_ROOT_NAME = "Singletons";
        #endregion
        #endregion
    }
}

