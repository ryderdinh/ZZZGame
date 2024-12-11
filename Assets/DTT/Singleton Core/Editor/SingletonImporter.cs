#if UNITY_EDITOR

using DTT.PublishingTools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

using Assembly = System.Reflection.Assembly;
using UnityAssembly = UnityEditor.Compilation.Assembly;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Responsible for managing the imports of singleton prefabs and scripts from the the asset database.
    /// </summary>
    public static class SingletonImporter
    {
        /// <summary>
        /// The guids of prefabs imported from the asset database.
        /// </summary>
        private static string[] _prefabGuids = Array.Empty<string>();

        /// <summary>
        /// The guids of scripts imported from the asset database.
        /// </summary>
        private static string[] _scriptGuids = Array.Empty<string>();

        /// <summary>
        /// The prefabs imported from the asset database.
        /// </summary>
        private static readonly List<GameObject> _importedPrefabs = new List<GameObject>();

        /// <summary>
        /// The scripts imported from the asset database.
        /// </summary>
        private static readonly List<Type> _importedScripts = new List<Type>();

        /// <summary>
        /// The mock progress shown when displaying the progress bar when
        /// doing heavy import operations.
        /// </summary>
        private const float MOCK_IMPORT_PROGRESS = 0.125f;
        
        /// <summary>
        /// Returns singleton prefabs imported from the project.
        /// </summary>
        /// <returns>The imported singleton prefabs.</returns>
        public static GameObject[] GetImportedPrefabs()
        {
            ImportPrefabs();

            return _importedPrefabs.ToArray();
        }

        /// <summary>
        /// Returns singleton scripts imported from the project.
        /// </summary>
        /// <returns>The imported singleton scripts.</returns>
        public static Type[] GetImportedScripts()
        {
            ImportScripts();

            return _importedScripts.ToArray();
        }

        /// <summary>
        /// Imports prefabs from the project and stores the ones with singleton components.
        /// </summary>
        private static void ImportPrefabs()
        {
            // Don't import anything if the configuration has not been loaded.
            SingletonConfigSO config = SingletonAssetDatabase.Config;
            if (config == null)
                return;
            
            // Only import prefabs in editor mode or in playmode when the 'playModeReload' flag is enabled.
            if (EditorApplication.isPlayingOrWillChangePlaymode && !config.playModeReload)
                return;

            string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" });
            if (guids.Length != _prefabGuids.Length || guids.Length != _importedPrefabs.Count)
            {
                // If the prefab asset count in the project has changed, refresh the imported prefabs list.
                _importedPrefabs.Clear();
                _prefabGuids = guids;


                if (config.useImportProgressPopup)
                {
                    EditorProgressBar.ShowMock(
                        "Fetching Prefabs",
                        "Retrieving singleton prefabs from the project.",
                        MOCK_IMPORT_PROGRESS,
                        AddImportedPrefabs
                    );
                }
                else
                {
                    AddImportedPrefabs();
                }

            }

            void AddImportedPrefabs()
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (prefab.tag != "EditorOnly" && SingletonValidator.HasSingletonComponent(prefab))
                        _importedPrefabs.Add(prefab);
                }
            }
        }

        /// <summary>
        /// Imports scripts from the project and stores the ones that are singleton components.
        /// </summary>
        private static void ImportScripts()
        {
            // Don't import anything if the configuration has not been loaded.
            SingletonConfigSO config = SingletonAssetDatabase.Config;
            if (config == null)
                return;
            
            // Only import scripts in editor mode or in playmode when the 'playModeReload' flag is enabled.
            if (EditorApplication.isPlayingOrWillChangePlaymode && !config.playModeReload)
                return;

            string[] guids = AssetDatabase.FindAssets("t:script", GetScriptSearchFolders());
            if (guids.Length != _scriptGuids.Length || guids.Length != _importedScripts.Count)
            {
                // If the script asset count has changed in the project or packages, refresh the imported scripts list. 
                _importedScripts.Clear();
                _scriptGuids = guids;

                if (config.useImportProgressPopup)
                {
                    EditorProgressBar.ShowMock(
                        "Fetching Scripts",
                        "Retrieving singleton scripts from the project.",
                        MOCK_IMPORT_PROGRESS,
                        AddImportedScripts
                    );
                }
                else
                {
                    AddImportedScripts();
                }
            }

            void AddImportedScripts()
            {
                AssembliesType assembliesType;
#if UNITY_2019_3_OR_NEWER
                assembliesType = AssembliesType.PlayerWithoutTestAssemblies;
#else
                assembliesType = AssembliesType.Player;
#endif
                UnityAssembly[] assemblies = CompilationPipeline.GetAssemblies(assembliesType);
                for (int i = 0; i < assemblies.Length; i++)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(assemblies[i].outputPath));
                        Type[] types = assembly.GetTypes();
                        for (int j = 0; j < types.Length; j++)
                        {
                            Type type = types[j];
                            // The type must not be a generic or nested type or a sub class of a singleton.
                            if (!type.IsGenericType && !type.IsNested && SingletonValidator.IsSubClassOfSingleton(type))
                                _importedScripts.Add(type);
                        }
                    }
                    catch
                    {
                        Debug.Log($"Singleton Core failed to import the assembly at path {assemblies[i].outputPath}.");
                    }
                }
            }
        }

        /// <summary>
        /// Returns an array of paths in which can be looked for scripts.
        /// </summary>
        /// <returns>The array of paths.</returns>
        private static string[] GetScriptSearchFolders()
        {
            string[] packageFolders = DTTEditorConfig.DTTPackageDirectories;
            string[] searchFolders = new string[packageFolders.Length + 1];
            searchFolders[0] = "Assets";

            Array.Copy(packageFolders, 0, searchFolders, 1, packageFolders.Length);

            return searchFolders;
        }
    }
}

#endif