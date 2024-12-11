#if UNITY_EDITOR

using DTT.Singletons.Exceptions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Serves as a backup to check whether a singleton profile is available inside a Resources
    /// folder in the project. 
    /// </summary>
    public class SingletonProfilePreProcessor : IPreprocessBuildWithReport
    {
        /// <summary>
        /// The order at which the OnPreprocessBuild function will be called in the case of 
        /// multiple IPreprocessBuildWithReport implementations. 
        /// </summary>
        /// <remarks> 
        /// The unity documentation: 
        /// https://docs.unity3d.com/ScriptReference/Build.IOrderedCallback-callbackOrder.html. 
        /// </remarks>
        public int callbackOrder => 0;
        
        /// <summary>
        /// Checks whether the singleton profile is available as an asset inside a resources folder.
        /// </summary>
        /// <param name="report">The report generated to give information about the build process.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            // A check on the singleton profile has to be done if we
            // can run a pre build check.
            if (CanRunPreBuildCheck())
                EnsureValidProfileUsage();
        }

        /// <summary>
        /// Returns whether the <see cref="SingletonConfigSO.runPreBuildCheck"/> flag was set in 
        /// the SingletonConfigSO asset.
        /// </summary>
        /// <returns>
        /// Whether the <see cref="SingletonConfigSO.runPreBuildCheck"/> flag was set in 
        /// the SingletonConfigSO asset. 
        /// </returns>
        private bool CanRunPreBuildCheck() 
            => SingletonAssetDatabase.Config != null && SingletonAssetDatabase.Config.runPreBuildCheck;

        /// <summary>
        /// Ensures that the profile is used correctly and will a 
        /// <see cref="InvalidProfileSetupException"/> if it hasn't.
        /// </summary>
        private void EnsureValidProfileUsage()
        {
            SingletonProfileSO profile = Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_ASSET_NAME);

            if (profile == null)
            {
                // If no profile exists but there are singleton prefabs in the project, 
                // an exception should be thrown to inform the user.                                 
                if (SingletonAssetDatabase.CanFindSingletonsInProject())
                    throw new InvalidProfileSetupException(
                        SingletonErrors.Get(SingletonError.SINGLETONS_WITHOUT_PROFILE_ON_BUILD));
            }
            else
            {
                // If a profile exists but there are no singletons inside, an exception should be 
                // thrown to inform the user.
                if (profile.Count == 0)
                    throw new InvalidProfileSetupException(
                       SingletonErrors.Get(SingletonError.PROFILE_WITHOUT_SINGLETONS_ON_BUILD));
            }
        }
    }
}

#endif
