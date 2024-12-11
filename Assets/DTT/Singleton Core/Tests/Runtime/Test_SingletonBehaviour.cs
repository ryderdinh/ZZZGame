#if TEST_FRAMEWORK

using DTT.Singletons.Exceptions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the singleton behaviour class.
    /// </summary>
    public class Test_SingletonBehaviour
    {
        #region InnerClasses
        /// <summary>
        /// A testable implementation of a singleton behaviour.
        /// </summary>
        private class TestableSingleton : SingletonBehaviour<TestableSingleton> { }
        #endregion

        #region Constructor
        /// <summary>
        /// Sets up the singleton behaviour tests by making sure the singleton profile is loaded
        /// and the singleton root game object is not active in the test scene. 
        /// </summary>
        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Make sure the singleton root is not already created.
            GameObject root = GameObject.Find(SingletonConfig.NONLAZY_ROOT_NAME);
            if (root != null)
                GameObject.Destroy(root);

            yield return null;
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the <see cref="SingletonBehaviour{T}"/> its exists property
        /// properly indicates whether the it is alive or not. It expects the property
        /// to return false when it doesn't yet exist and to return true when it does. 
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Exists()
        {
            // Exist should return false when lazy singleton is not yet existant.
            Assert.IsFalse(TestableSingleton.Exists, "Exists returns true before Singleton was created");

            // Create instance.
            TestableSingleton singletonInstance = new GameObject("TestSingleton").AddComponent<TestableSingleton>();

            // Exists should return true after the instance has been called.
            Assert.IsTrue(TestableSingleton.Exists, "Exists returns false after Singleton was created");

            // Destroy lazy singleton.
            singletonInstance.Destroy();
            yield return null; // Wait for OnDestroy to Run
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonBehaviour{T}"/> can be override by creating
        /// an additional singleton with the same component attached.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Integrity()
        {
            // Create first instance.
            TestableSingleton singletonInstance =
                new GameObject("TestSingleton").AddComponent<TestableSingleton>();

            // Create another instance.
            TestableSingleton anotherSingletonInstance =
                new GameObject("TestSingleton").AddComponent<TestableSingleton>();

            // The first singleton instance should be equal to the global testable singleton 
            // instance.
            Assert.AreEqual(singletonInstance, TestableSingleton.Instance,
                "The first created singleton instance was not equal to the global testable " +
                "singleton instance while it should");
            // The second singleton instance should not be equal to the global testable singleton
            // instance.
            Assert.AreNotEqual(anotherSingletonInstance, TestableSingleton.Instance,
                "The singleton created second was equal to the global testable singleton +" +
                "while it shouldn't");

            // Destroy singletons.
            Object.DestroyImmediate(singletonInstance);
            Object.DestroyImmediate(anotherSingletonInstance);
            yield return null; // Wait for OnDestroy to Run.
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonBehaviour{T}"/> is destoryed correctly.
        /// It expects the singleton instance retrieval to throw a <see cref="SingletonBootFailException"/>
        /// after its instance has been destroyed.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Destruction()
        {
            // Create instance.
            TestableSingleton singletonInstance = new GameObject("TestSingleton").AddComponent<TestableSingleton>();
            yield return null; // Wait for Awake to Run.
            // Destroy singleton.
            TestableSingleton.Instance.Destroy();
            yield return null; // Wait for OnDestroy to Run.

            TestDelegate action = () =>
            {
                var instance = TestableSingleton.Instance;
            };

            Assert.Catch<SingletonBootFailException>(action,
                "The singleton instance didn't throw an exception while it should have.");
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Removes testable singleton if it exists.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup()
        {
            if (TestableSingleton.Exists)
                TestableSingleton.Instance.Destroy();
        }
        #endregion
    }

}



#endif