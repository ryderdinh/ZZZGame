#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Watches asset imports and checks whether there are <see cref="SingletonBehaviour{T}"/> implementations
    /// that use a <see cref="BootModeAttribute"/>.
    /// </summary>
    public static class BootModeAttributeWatcher
    {
        /// <summary>
        /// A profile wrapper that accounts for the profile not being created.
        /// </summary>
        private class UnguaranteedProfile : Unguaranteed<SingletonProfileSO>
        {
            /// <summary>
            /// Whether the value has been accessed or not.
            /// </summary>
            public bool IsValueAccessed { get; private set; }

            /// <summary>
            /// Initializes the profile value.
            /// </summary>
            /// <param name="profileAssetPath">The path towards the profile asset.</param>
            public UnguaranteedProfile() : base(() => SingletonAssetDatabase.Profile)
            {
            }

            /// <summary>
            /// Sets the 'IsValueAccessed' flag to make saving of the profile possible.
            /// </summary>
            protected override void OnValueAccess()
            {
                base.OnValueAccess();

                IsValueAccessed = true;
            }
        }

        /// <summary>
        /// Gets imported singleton scripts and checks for each one whether it can be added to the profile
        /// based on whether they use a <see cref="BootModeAttribute"/>.
        /// </summary>
        [DidReloadScripts]
        public static void CheckProjectForAttributes()
        {
            // Don't do anything if the profile has not been loaded.
            if (SingletonAssetDatabase.Profile == null)
                return;
            
            Type[] scripts = SingletonImporter.GetImportedScripts();
            if (scripts.Length != 0)
            {
                // Setup the unguaranteed profile.
                UnguaranteedProfile profile = new UnguaranteedProfile();

                foreach (Type script in scripts)
                    TryAddSingletonScriptToProfile(script, profile);

                // If the profile has been used, set it dirty and save its changes.
                if (profile.IsValueAccessed)
                {
                    EditorUtility.SetDirty(profile.Value);
                    
                    // Use a delayed call to saving assets as saving instantly causes is a Unity-problem that
                    // will occur whenever an inspector previewing an asset is open while starting playmode.
                    EditorApplication.delayCall += SaveAssetAndRefresh;
                }
            }
        }

        /// <summary>
        /// Saves assets, refreshes the database and unsubscribes from the delayCall event.
        /// </summary>
        private static void SaveAssetAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorApplication.delayCall -= SaveAssetAndRefresh;
        }

        /// <summary>
        /// Tries adding or updating the profile with a singleton script based
        /// on whether the singleton script uses a <see cref="BootModeAttribute"/>.
        /// </summary>
        /// <param name="singletonScript">The singleton script.</param>
        /// <param name="unguaranteedProfile">The lazy profile.</param>
        private static void TryAddSingletonScriptToProfile(Type singletonScript, UnguaranteedProfile unguaranteedProfile)
        {
            if (!unguaranteedProfile.IsValueCreated)
                return;

            if (!TryGetBootModeFromType(singletonScript, out BootModeType bootModeType))
                return;
            
            SingletonProfileSO profile = unguaranteedProfile.Value;
            SingletonBootInfo info = profile.GetBootInfo(singletonScript);
            if (info != null)
            {
                profile.UpdateBootInfo(info.Prefab, singletonScript, bootModeType); 
            }
            else if(bootModeType != BootModeType.DISABLED)
            {
                // If the profile doesn't contain the type and it is not disabled, try finding the prefab 
                // it is attached to and use that to add it to the profile.
                GameObject prefab = SingletonAssetDatabase.FindSingletonPrefabFromType(singletonScript);
                if (prefab != null)
                {
                    profile.Add(prefab, singletonScript, bootModeType);
                }
                else
                {
                    Debug.LogWarning($"Found {singletonScript.Name} with boot mode {bootModeType} but it wasn't attached to a prefab. " +
                        "Make sure your singleton is attached to a prefab before adding the BootMode attribute.");
                }
            }
        }

        /// <summary>
        /// Outputs the boot mode type for a singleton type using the <see cref="BootModeAttribute"/>.
        /// <para>Will return false and output disabled if no boot mode attribute could be found.</para>
        /// </summary>
        /// <param name="singletonType">The type to get the boot mode type from.</param>
        /// <returns>Whether a bootmode type could be derived from the type.</returns>
        private static bool TryGetBootModeFromType(Type singletonType, out BootModeType bootModeType)
        {
            bootModeType = BootModeType.DISABLED;
            
            object[] attributes = singletonType.GetCustomAttributes(typeof(BootModeAttribute), false);
            if (attributes.Length == 0)
                return false;
            
            bootModeType = ((BootModeAttribute)attributes[0]).bootType;
            return true;
        }
    }
}

#endif