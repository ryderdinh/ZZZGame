#if UNITY_EDITOR

using DTT.Utils.EditorUtilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Provides asset database operations related to the singleton core package.
    /// </summary>
    public static class SingletonAssetDatabase
    {
        /// <summary>
        /// The singleton config scriptable object asset.
        /// </summary>
        public static SingletonConfigSO Config { get; private set; }
        
        /// <summary>
        /// The singleton profile scriptable object asset.
        /// </summary>
        public static SingletonProfileSO Profile { get; private set; }
        
        /// <summary>
        /// Initializes the configuration and profile assets.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void LoadConfigAndProfile()
        {
            GetOrCreateSingletonConfigAsset();
            GetOrCreateSingletonProfileAsset();
        }
        
        /// <summary>
        /// Creates the singleton config scriptable object asset inside the
        /// asset direction. Will just return the asset reference if it already exists.
        /// </summary>
        /// <returns>The singleton config asset.</returns>
        private static void GetOrCreateSingletonConfigAsset()
        {
            // Make sure the default asset path is created.
            string directoryPath = SingletonEditorConfig.ASSET_DIRECTORY_PATH;
            Directory.CreateDirectory(directoryPath);

            // Create or find the profile asset and save it.
            string assetPath = Path.Combine(directoryPath, SingletonEditorConfig.CONFIG_ASSET_NAME + ".asset");

            try
            {
                Config = AssetDatabaseUtility.GetOrCreateScriptableObjectAsset<SingletonConfigSO>(assetPath);

                // Save the asset database changes.
                AssetDatabase.SaveAssets();
            }
            catch (Exception)
            {
                // Configuration creation or assignment has failed. This can happen in case of project startup.
            }
        }

        /// <summary>
        /// Creates the singleton profile scriptable object asset at the default profile path.
        /// Will just return the asset reference if it already exists.
        /// </summary>
        /// <returns>The singleton profile.</returns>
        private static void GetOrCreateSingletonProfileAsset()
        {
            // Make sure the default asset directory is created.
            string directoryPath = SingletonEditorConfig.DEFAULT_PROFILE_DIRECTORY_PATH;
            Directory.CreateDirectory(directoryPath);

            // Create or find the profile asset and save it.
            string assetPath = Path.Combine(directoryPath, SingletonConfig.PROFILE_ASSET_NAME + ".asset");

            try
            {
                Profile = AssetDatabaseUtility.GetOrCreateScriptableObjectAsset<SingletonProfileSO>(assetPath);

                // Save the asset database changes.
                AssetDatabase.SaveAssets();
            }
            catch
            {
                // Profile creation or assignment has failed. This can happen in case of project startup.
            }
        }

        /// <summary>
        /// Returns whether there are singleton prefabs in the project.
        /// </summary>
        /// <returns>Whether there are singleton prefabs in the project.</returns>
        public static bool CanFindSingletonsInProject()
        {
            string[] guids = AssetDatabase.FindAssets("t:prefab");
            foreach (string guid in guids)
            {
                // If a prefab in the project has a component on it that is
                // a valid singleton type, return true.
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (SingletonValidator.HasSingletonComponent(prefab))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a singleton prefab with a singleton type attached.
        /// <para>Will return null if no prefab was found that had the singleton component type attached.</para>
        /// </summary>
        /// <param name="singletonType">The type of the singleton component that is attached.</param>
        /// <returns>The singleton prefab.</returns>
        public static GameObject FindSingletonPrefabFromType(Type singletonType)
        {
            GameObject[] prefabs = SingletonImporter.GetImportedPrefabs();
            foreach (GameObject prefab in prefabs)
            {
                if (SingletonValidator.TryGetSingletonType(prefab, out Type type) && type == singletonType)
                    return prefab;
            }

            return null;
        }
    }
}
#endif