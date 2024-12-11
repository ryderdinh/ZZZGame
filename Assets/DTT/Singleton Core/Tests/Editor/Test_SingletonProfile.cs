#if TEST_FRAMEWORK

using DTT.Singletons.Exceptions;
using NUnit.Framework;
using System;
using System.Reflection;
using UnityEngine;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the <see cref="SingletonProfileSO"/>.
    /// </summary>
    public class Test_SingletonProfile
    {
        /// <summary>
        /// The profile reference.
        /// </summary>
        private SingletonProfileSO _profile;

        [OneTimeSetUp]
        public void Setup() => _profile = ScriptableObject.CreateInstance<SingletonProfileSO>();

        /// <summary>
        /// Tests whether a valid entry added to the profile can be removed.
        /// It expects the valid entry to be removed from the profile.
        /// </summary>
        [Test]
        public void Test_Remove_Valid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.DEFAULT);

            Assert.NotZero(_profile.Count, "Expected the entry to be added but it wasn't.");

            // Act.
            _profile.Remove(gameObject);

            // Assert.
            Assert.Zero(_profile.Count, "Expected the entry to be removed but it wasn't.");

            // Cleanup.
            _profile.Clear();
        }


        /// <summary>
        /// Tests an invalid call for removal of a valid entry by the profile.
        /// It expects the profile not to removal the valid entry.
        /// </summary>
        [Test]
        public void Test_Remove_InValid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.DEFAULT);

            Assert.NotZero(_profile.Count, "Expected the entry to be added but it wasn't.");

            // Act.
            _profile.Remove(null);

            // Assert.
            Assert.NotZero(_profile.Count, "Expected the entry not to be removed but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether an entry with an invalid prefab can be added to the profile.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if that happens.
        /// </summary>
        [Test]
        public void Test_Add_Invalid_Prefab()
        {
            // Arrange.
            GameObject gameObject = null;
            Type type = typeof(SingletonOne);
            TestDelegate action = () =>
            {
                // Act.
                _profile.Add(gameObject, type, BootModeType.DEFAULT);
            };

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
        }


        /// <summary>
        /// Tests whether an entry with an invalid type can be added to the profile.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if that happens.
        /// </summary>
        [Test]
        public void Test_Add_Invalid_Type()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = null;
            TestDelegate action = () =>
            {
                // Act.
                _profile.Add(gameObject, type, BootModeType.DEFAULT);
            };

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether an entry with an invalid bootmode can be added to the profile.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if that happens.
        /// </summary>
        [Test]
        public void Test_Add_Invalid_BootMode()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            TestDelegate action = () =>
            {
                // Act.
                _profile.Add(gameObject, type, BootModeType.DISABLED);
            };

            // Assert.
            Assert.Catch<ArgumentException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether an entry with an invalid bootmode can be added to the profile.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if that happens.
        /// </summary>
        [Test]
        public void Test_Add_Invalid_Contains()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);

            TestDelegate action = () =>
            {
                // Act.
                _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            };

            // Assert.
            Assert.Catch<ArgumentException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether a valid entry can be added to the profile.
        /// It expects the valid entry to be added.
        /// </summary>
        [Test]
        public void Test_Add_Valid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);

            // Act.
            _profile.Add(gameObject, type, BootModeType.DEFAULT);

            // Assert.
            Assert.NotZero(_profile.Count, "Expected the entry to be added but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update its boot info.
        /// It expects an invalid prefab to cause an <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_UpdateBootInfo_Invalid_Prefab()
        {
            // Arrange.
            GameObject gameObject = null;
            Type type = typeof(SingletonOne);
            TestDelegate action = () =>
            {
                // Act.
                _profile.UpdateBootInfo(gameObject, type, BootModeType.DEFAULT);
            };

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
        }

        /// <summary>
        /// Tests whether the profile can properly update its boot info.
        /// It expects an invalid type to cause an <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_UpdateBootInfo_Invalid_Type()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = null;
            TestDelegate action = () =>
            {
                // Act.
                _profile.UpdateBootInfo(gameObject, type, BootModeType.DEFAULT);
            };

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an ArgumentNullException to be thrown but there wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }


        /// <summary>
        /// Tests whether the profile can properly update its boot info.
        /// It expects a boot mode of disabled to cause the entry to be removed.
        /// </summary>
        [Test]
        public void Test_UpdateBootInfo_Valid_Remove()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.LAZY);

            Assert.NotZero(_profile.Count, "Expected the entry to be added but it wasn't.");

            // Act.
            _profile.UpdateBootInfo(gameObject, type, BootModeType.DISABLED);

            // Assert.
            Assert.Zero(_profile.Count, "Expected the entry to be removed but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update its boot info.
        /// It expects a boot mode of lazy to be added to the profile if it wasn't yet there.
        /// </summary>
        [Test]
        public void Test_UpdateBootInfo_Valid_Add()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);

            // Act.
            _profile.UpdateBootInfo(gameObject, type, BootModeType.LAZY);

            // Assert.
            Assert.NotZero(_profile.Count, "Expected the entry to be added but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update its boot info.
        /// It expects the boot info to be updated if there was already a valid corresonding entry.
        /// </summary>
        [Test]
        public void Test_UpdateBootInfo_Valid_Update()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.LAZY);

            // Act.
            _profile.UpdateBootInfo(gameObject, type, BootModeType.DEFAULT);

            // Assert.
            BootModeType expected = BootModeType.DEFAULT;
            SingletonBootInfo info = _profile.GetBootInfo(type);

            Assert.NotNull(info, "Expected the profile to hold the added entry but it didn't.");
            Assert.AreEqual(expected, info.BootMode, "Expected the bootmode to be updated but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update a scene boot path for a singleton.
        /// It expects an invalid type to cause an <see cref="ArgumentException"/>.
        /// </summary>
        [Test]
        public void Test_UpdateSceneBootPathForSingleton_Invalid_Type()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.LAZY);
            Assert.NotZero(_profile.Count);

            TestDelegate action = () =>
            {
                // Act.
                _profile.UpdateSceneBootPathForSingleton(null, "path");
            };

            // Assert.
            Assert.Catch<ArgumentException>(action, "Expected the invalid type to cause an ArgumentException but it didn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update a scene boot path for a singleton.
        /// It expects an invalid scene path to cause an <see cref="ArgumentException"/>.
        /// </summary>
        [Test]
        public void Test_UpdateSceneBootPathForSingleton_Invalid_Scenepath()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.LAZY);
            Assert.NotZero(_profile.Count);

            TestDelegate action = () =>
            {
                // Act.
                _profile.UpdateSceneBootPathForSingleton(type, null);
            };

            // Assert.
            Assert.Catch<ArgumentException>(action, "Expected the null path to cause an ArgumentException but it didn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update a scene boot path for a singleton.
        /// It expects an invalid boot mode to cause an <see cref="InvalidProfileSetupException"/>.
        /// </summary>
        [Test]
        public void Test_UpdateSceneBootPathForSingleton_Invalid_BootMode()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.LAZY);
            Assert.NotZero(_profile.Count);

            TestDelegate action = () =>
            {
                // Act.
                _profile.UpdateSceneBootPathForSingleton(type, "Path");
            };

            // Assert.
            Assert.Catch<InvalidProfileSetupException>(action, "Expected the invalid boot mode of the entry to cause an exception but it didn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can properly update a scene boot path for a singleton.
        /// It expects a valid type and path to update the scene boot path for the singleton.
        /// </summary>
        [Test]
        public void Test_UpdateSceneBootPathForSingleton_Valid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            Assert.NotZero(_profile.Count);

            const string path = "path";

            // Act.
            _profile.UpdateSceneBootPathForSingleton(type, path);

            // Assert.
            SingletonBootInfo info = _profile.GetBootInfo(type);
            Assert.NotNull(info, "Expected the profile to hold the added entry but it didn't.");
            Assert.AreEqual(path, info.BootScenePath, "Expected the path to be updated but it wasn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can return boot info.
        /// It expects that if the given type is invalid, null will be returned.
        /// </summary>
        [Test]
        public void Test_GetBootInfo_Invalid_Type()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            Assert.NotZero(_profile.Count);

            // Act.
            SingletonBootInfo info = _profile.GetBootInfo(null);

            // Assert.
            Assert.IsNull(info, "Expected the invalid type to return a null boot info but it didn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// tests whether the profile can return boot info.
        /// It expects that if the given type is valid, a non-null value will be returned.
        /// </summary>
        [Test]
        public void Test_GetBootInfo_Valid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            Assert.NotZero(_profile.Count);

            // Act.
            SingletonBootInfo info = _profile.GetBootInfo(type);

            // Assert.
            Assert.NotNull(info, "Expected the invalid type to return a null boot info but it didn't.");

            // Cleanup.
            _profile.Clear();
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the profile can correctly detect entries in it.
        /// It expects a valid argument to return the true.
        /// </summary>
        [Test]
        public void Test_Contains_Valid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            Assert.NotZero(_profile.Count);

            // Act.
            bool contains = _profile.Contains(gameObject);

            // Assert.
            Assert.IsTrue(contains, "Expected the contains method to identify the type it held but it didn't.");
        }

        /// <summary>
        /// Tests whether the profile can correctly detect entries in it.
        /// It expects an invalid argumeent to return false.
        /// </summary>
        [Test]
        public void Test_Contains_Invalid()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            _profile.Add(gameObject, type, BootModeType.SCENE_BASED);
            Assert.NotZero(_profile.Count);

            // Act.
            bool contains = _profile.Contains(null);

            // Assert.
            Assert.IsFalse(contains, "Expected the contains method to identify the invalid type it didn't.");
        }

        /// <summary>
        /// Destroys the profile used for testing.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown() => GameObject.DestroyImmediate(_profile);
    }
}

#endif