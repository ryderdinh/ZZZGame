using DTT.Singletons.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// Stores the references to singleton prefabs that are going 
    /// to be created when the application is started.
    /// </summary>
    public class SingletonProfileSO : ScriptableObject, IEnumerable
    {
        /// <summary>
        /// Contains the singleton prefabs that are going to be created when the application is started.
        /// </summary>
        [SerializeField]
        private List<SingletonBootInfo> _bootInfo = new List<SingletonBootInfo>();

        /// <summary>
        /// The amount of singletons that can be booted.
        /// </summary>
        public int Count => _bootInfo.Count;
        
        /// <summary>
        /// Clears the profile.
        /// </summary>
        public void Clear() => _bootInfo.Clear();

        /// <summary>
        /// Removes given singleton prefab from the profile.
        /// </summary>
        /// <param name="singletonPrefab">The singleton prefab to remove.</param>
        public void Remove(GameObject singletonPrefab)
        {
            for (int i = _bootInfo.Count - 1; i >= 0; i--)
            {
                if (_bootInfo[i].Prefab == singletonPrefab)
                {
                    _bootInfo.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds the given singleton prefab to the profile.
        /// </summary>
        /// <param name="singletonPrefab">The singleton prefab to add.</param>
        /// <param name="singletonType">The type of the singleton.</param>
        /// <param name="bootMode">The initial boot mode for the singleton.</param>
        public void Add(GameObject singletonPrefab, Type singletonType, BootModeType bootMode)
        {
            if (singletonPrefab == null)
                throw new ArgumentNullException(nameof(singletonPrefab));

            if (singletonType == null)
                throw new ArgumentNullException(nameof(singletonType));

            if (bootMode == BootModeType.DISABLED)
                throw new ArgumentException(nameof(bootMode), "The profile can only contain bootable entries.");

            if (Contains(singletonPrefab))
                throw new ArgumentException("Singleton prefab was already part of the profile.");
            else
                _bootInfo.Add(new SingletonBootInfo(singletonPrefab, singletonType, bootMode));
        }

        /// <summary>
        /// Updates the profile with given boot info.
        /// </summary>
        /// <param name="singletonPrefab">The singleton prefab.</param>
        /// <param name="singletonType">The type of the singleton.</param>
        /// <param name="bootMode">The boot mode of the singleton.</param>
        public void UpdateBootInfo(GameObject singletonPrefab, Type singletonType, BootModeType bootMode)
        {
            if (bootMode != BootModeType.DISABLED)
            {
                if (Contains(singletonPrefab))
                {
                    // Update the boot info of the singleton if it is already stored.
                    SingletonBootInfo info = GetBootInfo(singletonType);
                    if (info == null)
                    {
                        Remove(singletonPrefab);
                        throw new InvalidProfileSetupException($"Failed finding boot info for singleton {singletonType.Name} " +
                               $"but the prefab {singletonPrefab.name} was part of it.");
                    }

                    info.BootMode = bootMode;

                    // Reset the boot scene path if the boot mode for the singleton is not scene based.
                    if (bootMode != BootModeType.SCENE_BASED)
                        info.BootScenePath = null;
                }
                else
                {
                    // Add the singleton prefab to the boot info if it isn't in it yet.
                    Add(singletonPrefab, singletonType, bootMode);
                }
            }
            else
            {
                // Remove the singleton prefab from the boot info if it has been disabled.
                Remove(singletonPrefab);
            }
        }

        /// <summary>
        /// Updates the scene boot path for a singleton in the profile.
        /// </summary>
        /// <param name="singletonPrefab">The singleton prefab.</param>
        /// <param name="singletonType">The type of the singleton.</param>
        /// <param name="scenePath">The path towards the scene in which the singleton will exist.</param>
        public void UpdateSceneBootPathForSingleton(Type singletonType, string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                throw new ArgumentException(nameof(scenePath));

            SingletonBootInfo info = GetBootInfo(singletonType);
            if (info == null)
                throw new ArgumentException($"Can't find boot info for {singletonType} singleton type.");

            if (info.BootMode != BootModeType.SCENE_BASED)
                throw new InvalidProfileSetupException($"Can't update scene boot path for {singletonType.Name} " +
                    "since its boot mode isn't scene based.");

            info.BootScenePath = scenePath;
        }

        /// <summary>
        /// Returns the boostrap info of a singleton in the profile.
        /// </summary>
        /// <param name="singletonType">The type of the singleton.</param>
        /// <returns>The singleton bootstrap info.</returns>
        public SingletonBootInfo GetBootInfo(Type singletonType)
        {
            for (int i = 0; i < _bootInfo.Count; i++)
            {
                SingletonBootInfo info = _bootInfo[i];
                if (info.SingletonTypeValue == singletonType)
                    return info;
            }

            return null;
        }

        /// <summary>
        /// Returns Whether the given singleton prefab is in the profile.
        /// </summary>
        /// <param name="singletonPrefab">The singleton prefab to check.</param>
        /// <returns>Wheter the given singleton prefab is in the profile.</returns>
        public bool Contains(GameObject singletonPrefab)
        {
            for (int i = 0; i < _bootInfo.Count; i++)
                if (_bootInfo[i].Prefab == singletonPrefab)
                    return true;

            return false;
        }

        /// <summary>
        /// Cleans the prefabs list of null entries.
        /// </summary>
        public void Cleanse()
        {
            for (int i = _bootInfo.Count - 1; i >= 0; i--)
                if (!_bootInfo[i].IsValid)
                    _bootInfo.RemoveAt(i);
        }

        /// <summary>
        /// Provides a way to iterate over stored prefab references inside the profile.
        /// </summary>
        /// <returns>The enumerator of the stored singleton prefabs to be booted</returns>
        public IEnumerator GetEnumerator() => _bootInfo.GetEnumerator();

        /// <summary>
        /// Cleans the prefabs list of null entries.
        /// </summary>
        private void OnEnable() => Cleanse();
    }
}
