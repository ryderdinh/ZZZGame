#if TEST_FRAMEWORK

using DTT.Singletons.Editor;
using DTT.Singletons.Exceptions;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the singleton profile pre processor on its job of enforcing good usage of
    /// the singleton profile.
    /// </summary>
    public class Test_SingletonProfilePreProcessor
    {
        #region Variables
        /// <summary>
        /// The singleton processor instance, used for calling the 
        /// <see cref="SingletonProfilePreProcessor.OnPreprocessBuild(BuildReport)"/>
        /// function.
        /// </summary>
        private SingletonProfilePreProcessor _processor;

        /// <summary>
        /// The singleton config scriptable object asset instance used for setting
        /// the <see cref="SingletonConfigSO.runPreBuildCheck"/> flag.
        /// </summary>
        private SingletonConfigSO _config;

        /// <summary>
        /// The value of the <see cref="SingletonConfigSO.runPreBuildCheck"/> flag
        /// on setup. This is stored to reset it back to its old value on cleanup.
        /// </summary>
        private bool _runPreBuildCheckOnSetup;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the processor instance.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _processor = new SingletonProfilePreProcessor();
            _config = SingletonAssetDatabase.Config;

            _runPreBuildCheckOnSetup = _config.runPreBuildCheck;
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the <see cref="SingletonProfilePreProcessor"/> cancels its operation
        /// if the PreRunBuildCheck flag isn't set. It expects the pre processor to not throw
        /// any exceptions when the flag is set but an exception should have been thrown.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_OnPreprocessBuild_UnsetPreRunBuildCheck()
        {
            // Make sure the RunPreBuildCheck flag is set to false.
            _config.runPreBuildCheck = false;

            // Make sure there is no profile in the project.
            SingletonProfileSO profile =
                Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);
            if (profile != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(profile));

            // Wait for the asset to be deleted.
            yield return null;

            // Create singleton with testable singleton component attached.
            const string name = "test-singleton";
            GameObject singleton = new GameObject(name);
            singleton.AddComponent<TestableSingleton>();

            // Save the singleton as a prefab so it can be found.
            string path = $"Assets/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(singleton, path, out bool succes);

            // Wait for the prefab to be created.
            yield return null;

            // Setup the test delegate.
            TestDelegate testDelegate = () => _processor.OnPreprocessBuild(null);

            // The delegate should not throw an InvalidProfileSetupException
            Assert.DoesNotThrow(testDelegate,
                $"The pre processor did throw an {nameof(InvalidProfileSetupException)}" +
                "exception while it shouldn't have.");

            // Clean up the singleton.
            GameObject.DestroyImmediate(singleton);
            AssetDatabase.DeleteAsset(path);

            yield return null;
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonProfilePreProcessor"/> throws an exception
        /// if the PreRunBuildCheck flag is set, the profile doesn't exist and singleton prefabs 
        /// can be found in the project. It expects the pre processor to throw an 
        /// <see cref="InvalidProfileSetupException"/>.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_OnPreprocessBuild_WillCatchInvalidProfileExceptionWithNoProfile()
        {
            // Make sure the RunPreBuildCheck flag is set to false.
            _config.runPreBuildCheck = true;

            // Make sure there is no profile in the project.
            SingletonProfileSO profile =
                Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);
            if (profile != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(profile));

            // Wait for the asset to be deleted.
            yield return null;

            // Create singleton with testable singleton component attached.
            const string name = "test-singleton";
            GameObject singleton = new GameObject(name);

            // Make sure it has a viable singleton component attached.
            singleton.AddComponent<TestableSingleton>();

            // Save the singleton as a prefab so it can be found.
            string path = $"Assets/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(singleton, path, out bool succes);

            // Wait for the prefab to be created.
            yield return null;

            // Setup the test delegate.
            TestDelegate testDelegate = () => _processor.OnPreprocessBuild(null);

            // The delegate should throw an InvalidProfileSetupException
            Assert.Catch<InvalidProfileSetupException>(testDelegate,
                $"The pre processor didn't throw an {nameof(InvalidProfileSetupException)}" +
                "exception while it should have.");

            // Clean up the singleton.
            GameObject.DestroyImmediate(singleton);
            AssetDatabase.DeleteAsset(path);

            yield return null;
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonProfilePreProcessor"/> throws an exception
        /// if the PreRunBuildCheck flag is set, the profile does exist but no singleton prefabs 
        /// can be found inside it. It expects the pre processor to throw an 
        /// <see cref="InvalidProfileSetupException"/>.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_OnPreprocessBuild_WillCatchInvalidProfileExceptionWithProfile()
        {
            // Make sure the RunPreBuildCheck flag is set to false.
            _config.runPreBuildCheck = true;

            // Make sure there is a profile in the project.
            SingletonProfileSO profile = SingletonAssetDatabase.Profile;

            // Setup the test delegate.
            TestDelegate testDelegate = () => _processor.OnPreprocessBuild(null);

            // The delegate should throw an InvalidProfileSetupException.
            Assert.Catch<InvalidProfileSetupException>(testDelegate,
                $"The pre processor didn't throw an {nameof(InvalidProfileSetupException)}" +
                "exception while it should have.");

            yield return null;
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Resets the <see cref="SingletonConfigSO.runPreBuildCheck"/>.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup()
        {
            _config.runPreBuildCheck = _runPreBuildCheckOnSetup;
        }
        #endregion
    }

}

#endif