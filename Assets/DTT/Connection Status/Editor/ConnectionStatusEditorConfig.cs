#if UNITY_EDITOR

using DTT.PublishingTools;
using System.IO;

namespace DTT.Networking.ConnectionStatus.Editor
{
    /// <summary>
    /// Holds the configuration values for the connection status package.
    /// </summary>
    internal static class ConnectionStatusEditorConfig
    {
        /// <summary>
        /// The location of the handler prefab.
        /// </summary>
        public static string HandlerPrefabLocation => Path.Combine(ResourcesFolder, WORKER_PREFAB_NAME);

        /// <summary>
        /// The resources folder used by the package.
        /// </summary>
        public static string ResourcesFolder => Path.Combine(ProjectFolder, "Resources");

        /// <summary>
        /// The project folder location.
        /// </summary>
        public static string ProjectFolder => Path.Combine(DTTEditorConfig.DTTProjectFolder, assetInfo.displayName);

        /// <summary>
        /// The name of the <see cref="InternetStatusWorker"/> prefab.
        /// </summary>
        public const string WORKER_PREFAB_NAME = "InternetStatusWorker.prefab";

        /// <summary>
        /// The name of the internet status window.
        /// </summary>
        public const string INTERNET_STATUS_WINDOW_NAME = "Internet Status Window";

        /// <summary>
        /// The menu item name of the internet status window.
        /// </summary>
        public const string INTERNET_STATUS_WINDOW_MENU_NAME = "Tools/DTT/ConnectionStatus/Window";

        /// <summary>
        /// The asset information of the connection status package.
        /// </summary>
        public readonly static AssetJson assetInfo;

        /// <summary>
        /// The full package name of the connection status package.
        /// </summary>
        private const string FULL_PACKAGE_NAME = "dtt.connectionstatus";


        /// <summary>
        /// Loads the asset info.
        /// </summary>
        static ConnectionStatusEditorConfig() => assetInfo = DTTEditorConfig.GetAssetJson(FULL_PACKAGE_NAME);
    }
}

#endif