#if TEST_FRAMEWORK

using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the <see cref="SingletonBootstrapService"/> class functionalities.
    /// </summary>
    public class Test_SingletonBootstrapService
    {
        #region InnerClasses
        /// <summary>
        /// A testable implementation of a singleton behaviour.
        /// </summary>
        private class TestableSingleton : SingletonBehaviour<TestableSingleton> { }
        #endregion

        #region Constructor
        /// <summary>
        /// Does the basic setup for the bootstrapper tests, making sure the profile is loaded and
        /// ensuring the singleton root object is not already part of the scene.
        /// </summary>
        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Make sure the singleton profile is loaded.
            SingletonProfileSO profile =
                Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);

            // Make sure the singleton root is not already created.
            GameObject root = GameObject.Find(SingletonConfig.NONLAZY_ROOT_NAME);
            if (root != null)
                GameObject.Destroy(root);

            yield return null;
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the bootstrap method will not create a singleton root object when there are
        /// no singleton prefabs to bootstrap. It expects there to be no singleton bootstrap object in
        /// the scene after calling the method. 
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Boostrap_NoPrefabs()
        {
            // Make sure the singleton profile is loaded.
            SingletonProfileSO profile =
                Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);

            if (profile == null)
            {
                Debug.LogWarning("There was no profile to test. Make sure there are singletons " +
                    "in the project so a profile can be created.");
                yield break;
            }

            // Make sure the singleton root is not already created
            GameObject root = GameObject.Find(SingletonConfig.NONLAZY_ROOT_NAME);
            if (root != null)
                GameObject.Destroy(root);

            yield return null;

            // Make sure no prefabs are inside the profile.
            profile.Clear();

            // Setup bootstrap method.        
            MethodInfo bootstrap = typeof(SingletonBootstrapService).
                GetMethod("Bootstrap", BindingFlags.NonPublic | BindingFlags.Static);

            // Call bootstrap.
            bootstrap.Invoke(null, null);

            // There should be no game object with the singleton root name inside the scene
            Assert.IsNull(GameObject.Find(SingletonConfig.NONLAZY_ROOT_NAME),
                "The singleton root shouldn't have been created in the scene but he was.");
        }
        #endregion
    }
}

#endif