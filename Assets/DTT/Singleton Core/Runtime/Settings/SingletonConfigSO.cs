using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// Contains all the singleton core settings open for change. 
    /// </summary>
    public class SingletonConfigSO : ScriptableObject
    {
        /// <summary>
        /// Whether there should be a check on the correct usage of the singleton profile
        /// during the build process. Setting this to true might stop the build if it the
        /// profile isn't used correctly. 
        /// </summary>
        [Tooltip("Whether there should be a check on the correct usage" +
            " of the singleton profile during the build process.")]
        public bool runPreBuildCheck = true;

        /// <summary>
        /// Whether the singleton profile window can refresh itself after the play mode assembly
        /// reload has finished. The reload is a heavy operation that delays the play mode start.
        /// If this is set to false, no info will be shown in the profile during play mode.
        /// </summary>
        [Tooltip("Whether the singleton profile window can refresh itself after the play mode assembly " +
            "reload has finished. Enabling this will delay the starting of playmode in larger projects.")]
        public bool playModeReload = true;

        /// <summary>
        /// Whether to show a progress popup when importing singletons for the profile to display.
        /// This can help when the import progress is delayed when having a bigger project.
        /// </summary>
        [Tooltip("Whether to show a progress popup when importing singletons for the " +
            "profile to display")]
        public bool useImportProgressPopup = false;
    }
}

