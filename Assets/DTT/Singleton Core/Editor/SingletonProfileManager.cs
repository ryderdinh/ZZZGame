#if UNITY_EDITOR

using DTT.Singletons.Exceptions;
using DTT.Singletons.Exceptions.Editor;
using DTT.Utils.EditorUtilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Can manage the <see cref="SingletonProfileSO"/> in the editor.
    /// </summary>
    public static class SingletonProfileManager
    {
        /// <summary>
        /// Whether there are prefabs currently stored.
        /// </summary>
        public static bool HasPrefabs => _prefabs.Count != 0;

        /// <summary>
        /// The singleton prefabs currently stored.
        /// </summary>
        public static SingletonPrefab[] Prefabs => _prefabs.ToArray();

        /// <summary>
        /// The singleton scripts currently stored.
        /// </summary>
        public static SingletonScript[] Scripts => _scripts.ToArray();

        /// <summary>
        /// The list storing the singleton prefabs.
        /// </summary>
        private static readonly List<SingletonPrefab> _prefabs = new List<SingletonPrefab>();

        /// <summary>
        /// The list storing the singleton scripts.
        /// </summary>
        private static readonly List<SingletonScript> _scripts = new List<SingletonScript>();

        /// <summary>
        /// A cached reference to the loaded singleton profile.
        /// </summary>
        private static SingletonProfileSO _profile;
        
        /// <summary>
        /// Creates the static singleton profile manager instance.
        /// </summary>
        static SingletonProfileManager()
        {
            _profile = SingletonAssetDatabase.Profile;
            _profile.Cleanse();
        }
        
        /// <summary>
        /// Refreshes the singleton prefabs list.
        /// </summary>
        public static void Refresh()
        {
            if (_profile == null)
            {
                Debug.LogWarning("Failed refreshing profile manager. No profile found.");
                return;
            }
            
            _prefabs.Clear();

            GameObject[] newPrefabs = SingletonImporter.GetImportedPrefabs();
            foreach (GameObject prefab in newPrefabs)
                TryAddPrefab(prefab);

            Type[] newScripts = SingletonImporter.GetImportedScripts();
            foreach (Type type in newScripts)
                TryAddScript(type);
        }

        /// <summary>
        /// Returns whether the given singleton prefab asset corresponds with a singleton 
        /// prefab that is enabled for bootstrap.
        /// </summary>
        /// <param name="singletonPrefabAsset">The singleton prefab asset to check.</param>
        /// <returns>Whether the given singleton prefab asset corresponds with a singleton 
        /// prefab that is enabled for bootstrap.</returns>
        public static bool IsEnabledForBootstrap(GameObject singletonPrefabAsset) =>
            _profile.Contains(singletonPrefabAsset);

        /// <summary>
        /// Returns whether the given singleton type corresponds with a singleton 
        /// prefab that is enabled for bootstrap.
        /// </summary>
        /// <param name="singletonType">The singleton type to check.</param>
        /// <returns>Whether the given singleton type corresponds with a singleton 
        /// prefab that is enabled for bootstrap.</returns>
        public static bool IsEnabledForBootstrap(Type singletonType) 
            => _profile.GetBootInfo(singletonType) != null;

        /// <summary>
        /// Returns whether the bootable prefabs list already
        /// contains a value using given singleton type.
        /// </summary>
        /// <param name="singletonType">The singleton type to check for.</param>
        /// <returns>Whether the bootable prefabs list 
        /// contains a value with given singleton type.</returns>
        public static bool ContainsPrefab(Type singletonType)
        {
            for (int i = 0; i < _prefabs.Count; i++)
                if (_prefabs[i].singletonType == singletonType)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns a singleton prefab in the profile with given prefab asset.
        /// </summary>
        /// <param name="prefabAsset">The prefab asset of the singleton prefab.</param>
        /// <returns>The singleton prefab in the profile.</returns>
        public static SingletonPrefab GetSingletonPrefab(GameObject prefabAsset)
        {
            for (int i = 0; i < _prefabs.Count; i++)
                if (_prefabs[i].asset == prefabAsset)
                    return _prefabs[i];

            return default;
        }

        /// <summary>
        /// Returns a singleton prefab in the profile with given singleton type.
        /// </summary>
        /// <param name="singletonType">The singleton type of the singleton prefab.</param>
        /// <returns>The singleton prefab in the profile.</returns>
        public static SingletonPrefab GetSingletonPrefab(Type singletonType)
        {
            for (int i = 0; i < _prefabs.Count; i++)
                if (_prefabs[i].singletonType == singletonType)
                    return _prefabs[i];

            return default;
        }

        /// <summary>
        /// Returns a singleton script for a given singleton script asset.
        /// </summary>
        /// <param name="scriptAsset">The singleton script asset of the singleton script.</param>
        /// <returns>The singleton script value.</returns>
        public static SingletonScript GetSingletonScript(TextAsset scriptAsset)
        {
            for (int i = 0; i < _scripts.Count; i++)
                if (_scripts[i].asset == scriptAsset)
                    return _scripts[i];

            return default;
        }

        /// <summary>
        /// Updates the profile with the enable value of given singleton prefab.
        /// </summary>
        /// <param name="prefab">The singleton prefab to update the profile with.</param>
        /// <param name="enable">The new enabled value.</param>
        public static void UpdateBootMode(SingletonPrefab prefab, BootModeType bootMode) => UpdateBootMode(prefab.asset, bootMode);

        /// <summary>
        /// Updates the profile with the enable value for singleton prefab with corresponding asset value.
        /// </summary>
        /// <param name="prefabAsset">The asset of which the singleton prefab entry to update.</param>
        /// <param name="enable">The new enabled value.</param>
        public static void UpdateBootMode(GameObject prefabAsset, BootModeType bootMode)
        {
            if (prefabAsset == null)
                throw new ArgumentNullException(nameof(prefabAsset), "Argument for updating the boot mode was null.");

            for (int i = _prefabs.Count - 1; i >= 0; i--)
            {
                SingletonPrefab bootablePrefab = _prefabs[i];
                // Check if a BootablePrefab's prefab field matches the given prefab.
                if (bootablePrefab.asset == prefabAsset)
                {
                    // Only update the entry if the enable value doesn't match.
                    if (bootablePrefab.BootMode != bootMode)
                    {
                        Type singletonType = _prefabs[i].singletonType;
                        _prefabs[i] = new SingletonPrefab(prefabAsset, bootMode, singletonType);

                        try
                        {
                            // Update the profile based on new boot mode value.
                            _profile.UpdateBootInfo(prefabAsset, singletonType, bootMode);

                            // Mark the profile as dirty so changes can be saved by the client.
                            EditorUtility.SetDirty(_profile);
                        }
                        catch (InvalidProfileSetupException e)
                        {
                            Debug.LogError(e.Message + " removing it from the profile.");
                            _prefabs.RemoveAt(i);
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Adds a scene to singleton prefab that is scene based.
        /// </summary>
        /// <param name="prefabAsset">The prefab asset of the singleton.</param>
        /// <param name="sceneAsset">The scene asset corresponding with the scene.</param>
        public static void AddSceneToSingletonPrefab(GameObject prefabAsset, SceneAsset sceneAsset)
        {
            if (prefabAsset == null)
                throw new ArgumentNullException(nameof(prefabAsset), "Argument for updating the boot mode was null.");

            if (sceneAsset == null)
                throw new ArgumentNullException(nameof(sceneAsset), "Argument for updating the boot mode was null.");

            for (int i = 0; i < _prefabs.Count; i++)
            {
                SingletonPrefab bootablePrefab = _prefabs[i];
                // Check if a BootablePrefab's prefab field matches the given prefab.
                if (bootablePrefab.asset == prefabAsset)
                {
                    if (bootablePrefab.BootMode == BootModeType.SCENE_BASED)
                    {
                        Type singletonType = _prefabs[i].singletonType;
                        _prefabs[i] = new SingletonPrefab(prefabAsset, sceneAsset, singletonType);

                        // Update the singleton in the profile with the scene asset path.
                        _profile.UpdateSceneBootPathForSingleton(singletonType, AssetDatabase.GetAssetPath(sceneAsset));

                        // Make sure the profile changes are saved.
                        EditorUtility.SetDirty(_profile);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        throw new InvalidOperationException("Tried updating scene for singleton prefab but it wasn't scene based.");
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Adds given prefab to the bootable prefabs list if it holds a valid singleton in the
        /// root game object component.
        /// </summary>
        /// <param name="prefab">The prefab to check.</param>
        /// <param name="enable">Whether or not the bootable should be enabled when added.</param>
        private static void TryAddPrefab(GameObject prefab)
        {
            if (prefab == null)
                return;

            // A bootable prefab can only be added if it isn't already stored and has a singleton
            // type component that isn't already on another stored singleton prefab.

            if (ContainsPrefab(prefab))
            {
                Debug.LogWarning($"A duplicate singleton prefab with name {prefab.name} has been detected. " +
                        "There can only be one singleton prefab in your project.");

                return;
            }

            if (SingletonValidator.TryGetSingletonType(prefab, out Type singletonType))
            {
                if (SingletonValidator.IsSubClassOfScriptSingleton(singletonType))
                {
                    Debug.LogWarning($"A singleton prefab with lazy singleton script {singletonType.Name} has been detected. " +
                        "Make sure it derives from SingletonBehaviour<> if you want it to use serialized fields.");
                    return;
                }

                if (ContainsPrefab(singletonType))
                {
                    Debug.LogWarning($"A singleton prefab with a duplicate script {singletonType.Name}  has been detected. " +
                        "There can only be one singleton component in your application.");

                    return;
                }

                UpdateProfileWithPrefabInfo(prefab, singletonType);
            }
        }

        /// <summary>
        /// Adds a new singleton script to the stored scripts list if it is valid.
        /// </summary>
        /// <param name="singletonType">The singleton type for the script.</param>
        private static void TryAddScript(Type singletonType)
        {
            if (singletonType == null)
                return;

            bool isRuntimeScript = SingletonValidator.IsSubClassOfScriptSingleton(singletonType);
            bool isEditorScript = EditorSingletonValidator.IsSubClassOfScriptSingleton(singletonType);
            if (!ContainsScript(singletonType) && (isRuntimeScript || isEditorScript))
            {
                TextAsset asset = AssetDatabaseUtility.LoadAsset<TextAsset>("t:script " + singletonType.Name);
                if (asset == null)
                    throw new SingletonImportException($"Failed loading text asset for {singletonType.Name}. Make sure the name of the script " +
                        "is the same as the name of the class.");

                _scripts.Add(new SingletonScript(singletonType, asset));
            }
        }

        private static void UpdateProfileWithPrefabInfo(GameObject prefab, Type singletonType)
        {
            // Use the boot info from the profile if it exists.
            // Otherwise the default boot mode is disabled.
            SingletonBootInfo bootInfo = _profile.GetBootInfo(singletonType);
            if (bootInfo != null)
            {
                // If the singleton had a stored boot scene path, use that to  
                // initialize the prefab.
                if (!string.IsNullOrEmpty(bootInfo.BootScenePath))
                {
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootInfo.BootScenePath);
                    _prefabs.Add(new SingletonPrefab(prefab, sceneAsset, singletonType));
                }
                else
                {
                    _prefabs.Add(new SingletonPrefab(prefab, bootInfo.BootMode, singletonType));
                }
            }
            else
            {
                _prefabs.Add(new SingletonPrefab(prefab, BootModeType.DISABLED, singletonType));
            }
        }
        
        /// <summary>
        /// Returns whether the bootable prefabs list 
        /// already contains a value using given prefab.
        /// </summary>
        /// <param name="singletonPrefabAsset">The singleton prefab asset to check.</param>
        /// <returns>Whether the bootable prefabs list contains 
        /// a value with given singleton prefab asset.</returns>
        private static bool ContainsPrefab(GameObject singletonPrefabAsset)
        {
            for (int i = 0; i < _prefabs.Count; i++)
                if (_prefabs[i].asset == singletonPrefabAsset)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns whether scripts list already
        /// contains a value using given singleton type.
        /// </summary>
        /// <param name="singletonType">The singleton type to check for.</param>
        /// <returns>
        /// Whether the scripts list contains
        /// a value with given singleton type.
        /// </returns>
        private static bool ContainsScript(Type singletonType)
        {
            for (int i = 0; i < _scripts.Count; i++)
                if (_scripts[i].singletonType == singletonType)
                    return true;

            return false;
        }
    }
}

#endif