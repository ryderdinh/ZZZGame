#if UNITY_EDITOR

using DTT.PublishingTools;
using System.IO;

namespace DTT.Singletons
{
    /// <summary>
    /// Holds all configuration data related to the singleton core package in the ditor. Make sure 
    /// to use the strings stored here for all editor singleton-core related operations.
    /// </summary>
    public class SingletonEditorConfig
    {
        #region Variables
        #region Public
        /// <summary>
        /// The title of the singleton profile window in the inspector.
        /// </summary>
        public const string PROFILE_WINDOW_TITLE = SingletonConfig.PROFILE_ASSET_NAME;

        /// <summary>
        /// The folder in which the singleton profile is created if it
        /// can't be loaded from the <see cref="PROFILE_LOAD_PATH"/>.
        /// </summary>
        public static string DEFAULT_PROFILE_DIRECTORY_PATH => Path.Combine(ASSET_DIRECTORY_PATH, "Resources");

        /// <summary>
        /// The folder in which all SingletonCore assets are stored in the project.
        /// <para>Internal asset name differs from asset store release.</para>
        /// </summary>
        public static string ASSET_DIRECTORY_PATH => Path.Combine(
            "Assets",
            "DTT",
            "Singleton Core"
        );

        /// <summary>
        /// The art folder for this package.
        /// </summary>
        public static string ArtFolder => Path.Combine(DTTEditorConfig.GetContentFolderPath(assetInfo), "Editor", "Art");

        /// <summary>
        /// The name of the scriptable object configuration containing 
        /// all the singleton core settings open for change. 
        /// </summary>
        public const string CONFIG_ASSET_NAME = "SingletonConfig";

        /// <summary>
        /// The title of the singleton configuration window in the inspector.
        /// </summary>
        public const string CONFIG_WINDOW_TITLE = CONFIG_ASSET_NAME;

        /// <summary>
        /// The full name of this package used by npm.
        /// </summary>
        public const string FULL_PACKAGE_NAME = "dtt.singletoncore";

        /// <summary>
        /// The singleton core asset json file container.
        /// </summary>
        public static readonly AssetJson assetInfo;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the static instance, storing the asset json information.
        /// </summary>
        static SingletonEditorConfig() => assetInfo = DTTEditorConfig.GetAssetJson("dtt.singletoncore");
        #endregion
    }
}

#endif