#if TEST_FRAMEWORK

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the lazy singleton behaviour
    /// </summary>
    [TestFixture]
    public class Test_LazySingletonBehaviour
    {
        #region InnerClasses
        /// <summary>
        /// A testable class that implements the lazy singleton behaviour class
        /// </summary>
        private class TestableLazySingleton : LazySingletonBehaviour<TestableLazySingleton>
        {
            public void AutoInstantiateFunction() { }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Removes testable lazy singleton if it exists.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            if (TestableLazySingleton.Exists)
                TestableLazySingleton.Instance.Destroy();
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the <see cref="LazySingletonBehaviour{T}"/> will automatically
        /// create itself when its instance is called. It expects it to be created after
        /// the instance has been called.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_AutoInstantiation()
        {
            // Create instance by calling it.
            TestableLazySingleton singletonInstance = TestableLazySingleton.Instance;

            // Instance should now not be null.
            Assert.IsNotNull(singletonInstance, "Instance was not auto instantiated");

            // Destroy lazy singleton.
            TestableLazySingleton.Instance.Destroy();
            yield return null; // Wait for OnDestroy to Run.
        }

        /// <summary>
        /// Tests whether the <see cref="LazySingletonBehaviour{T}"/> its exists property
        /// properly indicates whether the it is alive or not. It expects the property
        /// to return false when it doesn't yet exist and to return true when it does. 
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Exists()
        {
            // Exist should return false when lazy singleton is not yet existant.
            Assert.IsFalse(TestableLazySingleton.Exists, "Exists returns true before Singleton was created");

            // Create instance by calling it.
            TestableLazySingleton singletonInstance = TestableLazySingleton.Instance;

            // Exists should return true after the instance has been called.
            Assert.IsTrue(TestableLazySingleton.Exists, "Exists returns false after Singleton was created");

            // Destroy lazy singleton.
            TestableLazySingleton.Instance.Destroy();
            yield return null; // Wait for OnDestroy to Run
        }

        /// <summary>
        /// Tests whether the <see cref="LazySingletonBehaviour{T}"/> can be override by creating
        /// an additional singleton with the same component attached.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Integrity()
        {
            // Create first instance.
            TestableLazySingleton singletonInstance = TestableLazySingleton.Instance;

            // Create another instance.
            TestableLazySingleton anotherSingletonInstance =
                new GameObject("TestSingleton").AddComponent<TestableLazySingleton>();

            // The first singleton instance should be equal to the global testable singleton 
            // instance.
            Assert.AreEqual(singletonInstance, TestableLazySingleton.Instance,
                "The first created singleton instance was not equal to the global testable " +
                "singleton instance while it should");
            // The second singleton instance should not be equal to the global testable singleton
            // instance.
            Assert.AreNotEqual(anotherSingletonInstance, TestableLazySingleton.Instance,
                "The singleton created second was equal to the global testable singleton +" +
                "while it shouldn't");

            // Destroy singletons.
            Object.DestroyImmediate(singletonInstance);
            Object.DestroyImmediate(anotherSingletonInstance);
            yield return null; // Wait for OnDestroy to Run.
        }

        /// <summary>
        /// Tests whether the <see cref="LazySingletonBehaviour{T}"/> is destoryed correctly.
        /// It expects the singleton instance to be null after the destroy method has been
        /// called.
        /// </summary>
        [UnityTest]
        public IEnumerator DestructionTest()
        {
            // Create instance by calling it.
            TestableLazySingleton singletonInstance = TestableLazySingleton.Instance;
            yield return null; // Wait for Awake to Run.
            // Destroy singleton.
            TestableLazySingleton.Instance.Destroy();
            yield return null; // Wait for OnDestroy to Run.
            //The singleton should not exist anymore
            Assert.IsFalse(TestableLazySingleton.Exists, "singleton was not properly destroyed");
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Removes testable lazy singleton if it exists.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup()
        {
            if (TestableLazySingleton.Exists)
                TestableLazySingleton.Instance.Destroy();
        }
        #endregion
    }
}



#endif