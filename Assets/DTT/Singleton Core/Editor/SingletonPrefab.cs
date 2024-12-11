#if UNITY_EDITOR

using DTT.Utils.Extensions;
using System;
using UnityEditor;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Contains the information regarding a singleton that can be part of the 
    /// application bootstrap process.
    /// </summary>
    public struct SingletonPrefab
    {
        #region Constructor
        /// <summary>
        /// Initializes the bootable prefab with its required data.
        /// </summary>
        /// <param name="asset">The prefab asset in the project.</param>
        /// <param name="bootMode">The type of boot the singleton will use.</param>
        /// <param name="singletonType">
        /// The type of singleton that is 
        /// attached to the singleton prefab.
        /// </param>
        public SingletonPrefab(GameObject asset, BootModeType bootMode, Type singletonType)
            : this(asset, bootMode, singletonType, null)
        {
        }

        /// <summary>
        /// Initializes the bootable prefab with its required data.
        /// </summary>
        /// <param name="asset">The prefab asset in the project.</param>
        /// <param name="bootMode">The type of boot the singleton will use.</param>
        /// <param name="singletonType">
        /// The type of singleton that is 
        /// attached to the singleton prefab.
        /// </param>
        public SingletonPrefab(GameObject asset, SceneAsset sceneAsset, Type singletonType)
            : this(asset, BootModeType.SCENE_BASED, singletonType, sceneAsset)
        {
        }

        /// <summary>
        /// Initializes the bootable prefab with its required data.
        /// </summary>
        /// <param name="asset">The prefab asset in the project.</param>
        /// <param name="bootMode">The type of boot the singleton will use.</param>
        /// <param name="singletonType">
        /// The type of singleton that is 
        /// attached to the singleton prefab.
        /// </param>
        /// <param name="sceneAsset">The scene asset used for a scene based boot mode.</param>
        private SingletonPrefab(GameObject asset, BootModeType bootMode, Type singletonType, SceneAsset sceneAsset)
        {
            this.asset = asset;
            this.singletonType = singletonType;
            this.sceneAsset = sceneAsset;
            this.BootMode = bootMode;

            // Assign a bootmode based on the bootmode attribute if it has been used.
            object[] attributes = singletonType.GetCustomAttributes(typeof(BootModeAttribute), false);
            BootModeAttribute attribute = attributes.Length != 0 ? (BootModeAttribute)attributes[0] : null;
            this.usesAttribute = attribute != null;
            if (this.usesAttribute)
                BootMode = attribute.bootType;
        }
        #endregion

        #region Variables
        #region Public
        /// <summary>
        /// The prefab asset in the project.
        /// </summary>
        public readonly GameObject asset;

        /// <summary>
        /// The scene asset used for a scene based boot mode.
        /// </summary>
        public readonly SceneAsset sceneAsset;

        /// <summary>
        /// The type of singleton component that is attached to the singleton prefab.
        /// </summary>
        public readonly Type singletonType;

        /// <summary>
        /// Whether a <see cref="BootModeAttribute"/> is used by the prefab.
        /// </summary>
        public readonly bool usesAttribute;

        /// <summary>
        /// Is the prefab going to be instantiated during the application lifetime.
        /// </summary>
        public bool Enabled => BootMode != BootModeType.DISABLED;

        /// <summary>
        /// The boot mode used for this prefab.
        /// </summary>
        public BootModeType BootMode { get; private set; }

        /// <summary>
        /// Whether the singleton prefab is valid or not.
        /// </summary>
        public bool IsValid => asset != null && singletonType != null;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Cycles the boot mode to the next value. Will return back to 
        /// the first value if there is none.
        /// </summary>
        public void CycleBootMode() => BootMode = BootMode.Next();
        #endregion
    }
}

#endif