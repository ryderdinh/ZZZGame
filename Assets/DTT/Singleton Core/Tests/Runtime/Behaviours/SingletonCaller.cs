#if TEST_FRAMEWORK

using NUnit.Framework;
using UnityEngine;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// A behaviour used for calling upon the test singletons in its Awake method.
    /// </summary>
    public class SingletonCaller : MonoBehaviour
    {
        /// <summary>
        /// Asserts that all instance values retrieved from all test singletons are not null.
        /// </summary>
        private void Awake()
        {
            Assert.NotNull(SingletonTwo.Instance, "Expected the singleton two instance to exist in Awake but it didn't.");
            Assert.NotNull(SingletonThree.Instance, "Expected the singleton three instance to exist in Awake but it didn't.");
            Assert.NotNull(SingletonOne.Instance, "Expected the singleton one instance to exist in Awake but it didn't.");
        }
    }
}
#endif