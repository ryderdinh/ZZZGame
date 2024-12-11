#if TEST_FRAMEWORK

using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Networking.ConnectionStatus.Tests
{
    /// <summary>
    /// Tests the <see cref="InternetStatusManager"/> class.
    /// </summary>
    public class Test_InternetStatusManager
    {
        /// <summary>
        /// The binding flags used for the current status field.
        /// </summary>
        private const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

        /// <summary>
        /// The field info used for updating the current status field of the target.
        /// </summary>
        private static readonly FieldInfo _currentStatusInfo =
            typeof(InternetStatusTarget).GetField("_currentStatus", FLAGS);

        /// <summary>
        /// Destroys the internet status handler if it exists and sets up reflection cache.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            if(InternetStatusManager.Worker == null)
                Assert.Fail("Worked was not instantiated before the test scene was loaded.");
        }

        /// <summary>
        /// Tests whether a name or address can be used to find a target.
        /// It expected an invalid name to not find any target.
        /// </summary>
        [Test]
        public void Test_GetTarget_NameOrAdress_NotFound()
        {
            // Arrange.
            string name = "InvalidName";
            
            // Act.
            InternetStatusTarget target = InternetStatusManager.GetTarget(name);
            
            // Assert.
            Assert.IsNull(target, "Expected the invalid name to return a null target but it didn't.");
        }
        
        /// <summary>
        /// Tests whether a name or address can be used to find a target.
        /// It expects a valid name used to find a valid target.
        /// </summary>
        [Test]
        public void Test_GetTarget_Name_Found()
        {
            // Arrange.
            string name = InternetStatusTarget.DEFAULT_NAME;
            
            // Act.
            InternetStatusTarget target = InternetStatusManager.GetTarget(name);
            
            // Assert.
            Assert.IsNotNull(target, "Expected the valid name to return a valid target but it didn't.");
        }

        /// <summary>
        /// Tests whether a name or address can be used to find a target.
        /// It expects a valid address used to find a valid target.
        /// </summary>
        [Test]
        public void Test_GetTarget_Address_Found()
        {
            // Arrange.
            string address = InternetStatusTarget.DEFAULT_ADDRESS;
            
            // Act.
            InternetStatusTarget target = InternetStatusManager.GetTarget(address);
            
            // Assert.
            Assert.IsNotNull(target, "Expected the valid address to return a valid target but it didn't.");
        }
        
        /// <summary>
        /// Tests whether an index can be used to find a target.
        /// It expects a valid index used to return a valid target.
        /// </summary>
        [Test]
        public void Test_GetTarget_Index()
        {
            // Arrange.
            int index = 0;
            
            // Act.
            InternetStatusTarget target = InternetStatusManager.GetTarget(index);
            
            // Assert.
            Assert.IsNotNull(target, "Expected the valid index to return a valid target but it didn't.");
        }

        /// <summary>
        /// Tests whether an index can be used to find a target.
        /// It expects a <see cref="ArgumentOutOfRangeException"/> if the index is out of bounds.
        /// </summary>
        [Test]
        public void Test_GetTarget_Out_Of_Bounds()
        {
            // Arrange.
            int index = 5;
            
            // Act.
            TestDelegate action = () => InternetStatusManager.GetTarget(index);
            
            // Assert.
            Assert.Catch<ArgumentOutOfRangeException>(action,
                "Expected the invalid index to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the default target is correctly based on the set default target index.
        /// It expects the default target to be the same as the one found with the default target index.
        /// </summary>
        [Test]
        public void Test_Default_Target()
        {
            // Arrange.
            int defaultTargetIndex = InternetStatusManager.Worker.DefaultTargetIndex;
            
            // Act.
            InternetStatusTarget actual = InternetStatusManager.DefaultTarget;
            InternetStatusTarget expected = InternetStatusManager.GetTarget(defaultTargetIndex);
            
            // Assert.
            Assert.AreEqual(actual.Name, expected.Name,
                "Expected the default target to be based on the default target index but it wasn't.");
        }

        /// <summary>
        /// Tests whether attempting to reconnect is done correctly.
        /// It expects the callback to go through after the maximum reconnect duration if none is given.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_AttemptReconnect_Online()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            
            // Act.
            bool isOnline = false;
            target.AttemptReconnect((value) => isOnline = value);

            yield return new WaitForSeconds(target.MaxReconnectDuration + 0.1f);
            
            // Assert.
            Assert.IsTrue(isOnline, "Expected to be online after reconnect but we weren't.");
        }
        
        /// <summary>
        /// Tests whether attempting to reconnect is done correctly.
        /// It expects the callback to go through after the maximum reconnection duration if none is given.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_AttemptReconnect_Offline()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            _currentStatusInfo.SetValue(target,InternetStatus.OFFLINE);
            
            // Act.
            bool isOnline = false;
            target.AttemptReconnect((value) => isOnline = value);

            yield return new WaitForSeconds(target.MaxReconnectDuration + 0.1f);
            
            // Assert.
            Assert.IsTrue(isOnline, "Expected to be online after reconnect but we weren't.");
        }
        
        /// <summary>
        /// Tests whether attempting to reconnect is done correctly.
        /// It expects the target to be online after the maximum duration has finished.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_AttemptReconnect_No_Callback_Online()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;

            // Act.
            target.AttemptReconnect();

            yield return new WaitForSeconds(target.MaxReconnectDuration + 0.1f);
            
            // Assert.
            Assert.IsTrue(target.IsOnline, "Expected to be online after reconnect but we weren't.");
        }
        
        /// <summary>
        /// Tests whether attempting to reconnect is done correctly.
        /// It expects the target to be online after the given minimum duration has finished.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_AttemptReconnect_Override_MinDuration_Online()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;

            // Act.
            float minDuration = 1f;
            bool isOnline = false;
            target.AttemptReconnect(minDuration,(value) => isOnline = value);

            yield return new WaitForSeconds(minDuration + 0.1f);
            
            // Assert.
            Assert.IsTrue(isOnline, "Expected to be online after reconnect but we weren't.");
        }

        /// <summary>
        /// Tests whether the user correctly gets a callback on a given status.
        /// It expects the callback to be instant if the given status is already active.
        /// </summary>
        [Test]
        public void Test_On_Instant_Callback()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            _currentStatusInfo.SetValue(target,InternetStatus.ONLINE);
            
            // Act.
            bool isOnline = false;
            target.On(InternetStatus.ONLINE,() => isOnline = true);
            
            // Assert.
            Assert.IsTrue(isOnline, "Expected the online callback to be instant but it wasn't.");
        }

        /// <summary>
        /// Tests whether the user correctly gets a callback on a given status.
        /// It expects the callback to be be done after the targets maximum duration if
        /// the status is currently offline but will become online.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_On_Offline_Callback()
        {
            // Skip this test if the internet is not reachable.
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            _currentStatusInfo.SetValue(target,InternetStatus.OFFLINE);

            // Act.
            bool isOnline = false;
            target.On(InternetStatus.ONLINE,() => isOnline = true);
            
            Assert.IsFalse(isOnline, "Expected the online callback not to be instant but it was.");

            yield return new WaitForSeconds(target.MaxReconnectDuration);
            
            // Assert.
            Assert.IsTrue(isOnline, "Expected the online callback to be made but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether the target can correctly return whether it is online.
        /// It expects the target to correctly return whether it is online or not.
        /// </summary>
        [Test]
        public void Test_IsOnline()
        {
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            
            // Act.
            _currentStatusInfo.SetValue(target,InternetStatus.ONLINE);
            
            // Assert.
            Assert.IsTrue(target.IsOnline, "Expected the target to be offline but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether the target can correctly callback when it become online.
        /// It expects the target to correctly callback when it becomes online.
        /// </summary>
        [Test]
        public void Test_OnOnline()
        {
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;

            // Act.
            bool callbacked = false;
            target.OnOnline += () => callbacked = true;
            
            target.UpdateStatus(InternetStatus.ONLINE, true);

            // Assert. 
            Assert.IsTrue(callbacked, "Expected the callback to go through but it didn't.");
        }

        /// <summary>
        /// Tests whether the target can correctly return whether it is offline.
        /// It expects the target to correctly return whether it is offline or not.
        /// </summary>
        [Test]
        public void Test_IsOffline()
        {
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;
            
            // Act.
            _currentStatusInfo.SetValue(target,InternetStatus.OFFLINE);
            
            // Assert.
            Assert.IsTrue(target.IsOffline, "Expected the target to be offline but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether the target can correctly callback when it become offline.
        /// It expects the target to correctly callback when it becomes offline.
        /// </summary>
        [Test]
        public void Test_OnOffline()
        {
            // Arrange.
            InternetStatusTarget target = InternetStatusManager.DefaultTarget;

            // Act.
            bool callbacked = false;
            target.OnOffline += () => callbacked = true;
            
            target.UpdateStatus(InternetStatus.OFFLINE, true);

            // Assert.
            Assert.IsTrue(callbacked, "Expected the callback to go through but it didn't.");
        }

    }
}

#endif