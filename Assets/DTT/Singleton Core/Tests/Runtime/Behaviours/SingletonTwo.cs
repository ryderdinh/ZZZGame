#if TEST_FRAMEWORK

using NUnit.Framework;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// A testable singleton class.
    /// </summary>
    public class SingletonTwo : SingletonBehaviour<SingletonTwo>
    {
        /// <summary>
        /// Asserts that the other test singleton instances are not null.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Assert.NotNull(SingletonThree.Instance, "Expected the singleton three instance to exist in Awake but it didn't.");
            Assert.NotNull(SingletonOne.Instance, "Expected the singleton one instance to exist in Awake but it didn't.");
        }

        /// <summary>
        /// Asserts that the other test singleton instances are not null.
        /// </summary>
        private void OnEnable()
        {
            Assert.NotNull(SingletonThree.Instance, "Expected the singleton three instance to exist in OnEnable but it didn't.");
            Assert.NotNull(SingletonOne.Instance, "Expected the singleton one instance to exist in OnEnable but it didn't.");
        }
    }
}

#endif