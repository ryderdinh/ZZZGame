#if TEST_FRAMEWORK

using DTT.Singletons.Exceptions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the <see cref="SingletonBehaviour{T}{T}"/> in the editor.
    /// </summary>
    public class Test_SingletonBehaviour_Editor
    {
        #region InnerClasses
        /// <summary>
        /// A testable lazy singleton.
        /// </summary>
        public class TestSingleton : SingletonBehaviour<TestSingleton> { }
        #endregion

        #region Tests
        /// <summary>
        /// Tests the usage of the <see cref="SingletonBehaviour{T}"/> in the editor.
        /// It expects a <see cref="SingletonCreationException"/> to be thrown when the
        /// instance is accessed in editor mode.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_EditorUsage()
        {
            GameObject gameObject = new GameObject("test_singleton");
            gameObject.AddComponent<TestSingleton>();

            yield return null;

            TestDelegate action = () =>
            {
                TestSingleton instance = TestSingleton.Instance;
            };

            Assert.Catch<SingletonInstanceException>(action,
                "Expected a SingletonInstanceException to occur while accessing the instance in the editor but it didn't.");
        }
        #endregion
    }
}
#endif