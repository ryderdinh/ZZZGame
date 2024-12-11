#if TEST_FRAMEWORK

using NUnit.Framework;
using System;
using System.Reflection;
using UnityEngine;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the <see cref="SingletonBootInfo"/> class.
    /// </summary>
    public class Test_SingletonBootInfo
    {
        /// <summary>
        /// Tests whether the 'IsBootstrappable' flag returns the correct true value.
        /// </summary>
        [Test]
        public void Test_IsBootstrappable_True()
        {
            SingletonBootInfo info = new SingletonBootInfo(null, typeof(SingletonOne), BootModeType.DEFAULT);
            Assert.IsTrue(info.IsBootstrappable, "Expected the boot info to be bootstrappable based on the boot mode but it wasn't.");
        }

        /// <summary>
        /// Tests whether the 'IsBootstrappable' flag returns the correct false value.
        /// </summary>
        [Test]
        public void Test_IsBootstrappable_False()
        {
            SingletonBootInfo info = new SingletonBootInfo(null, typeof(SingletonOne), BootModeType.LAZY);
            Assert.IsFalse(info.IsBootstrappable, "Expected the boot info not to be bootstrappable based on the boot mode but it was.");
        }

        /// <summary>
        /// Tests whether the existence is properly reset.
        /// It expects the 'exists' flag to be reset to its default value.
        /// </summary>
        [Test]
        public void Test_Reset_Existance()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            SingletonBootInfo info = new SingletonBootInfo(gameObject, type, BootModeType.LAZY);

            MethodInfo setter = typeof(SingletonBootInfo).GetProperty("Exists").GetSetMethod(true);
            setter.Invoke(info, new object[] { true });
            Assert.IsTrue(info.Exists);

            // Act.
            info.Reset();

            // Assert.
            Assert.IsFalse(info.Exists, "Expected the reset method to reset the exists flag but it didn't.");
        }

        /// <summary>
        /// Tests whether the scene object is properly reset.
        /// It expects the scene object to be reset to its default value.
        /// </summary>
        [Test]
        public void Test_Reset_SceneObject()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            Type type = typeof(SingletonOne);
            SingletonBootInfo info = new SingletonBootInfo(gameObject, type, BootModeType.LAZY);

            MethodInfo setter = typeof(SingletonBootInfo).GetProperty("SceneObject").GetSetMethod(true);
            setter.Invoke(info, new object[] { gameObject });
            Assert.NotNull(info.SceneObject);

            // Act.
            info.Reset();

            // Assert.
            Assert.IsNull(info.SceneObject, "Expected the reset method to reset the scene object but it didn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonBootInfo"/> can properly identify whether it is valid or not.
        /// It expects that if the prefab is null, it returns as invalid.
        /// </summary>
        [Test]
        public void Test_IsValid_Null_Prefab()
        {
            // Arrange.
            SingletonBootInfo info = new SingletonBootInfo(null, typeof(SingletonOne), BootModeType.DEFAULT);

            // Act.
            bool condition = info.IsValid;

            // Assert.
            Assert.IsFalse(condition, "Expected a singleton boot info with a null prefab to not be valid but it was.");
        }

        // DEVNOTE: This test will be enabled when Editor Utilities has fixed the null reference exception error regarding serialized null types.
        //[Test]
        //public void Test_IsValid_Null_SingletonType()
        //{
        //    // Arrange.
        //    GameObject gameObject = new GameObject();
        //    SingletonBootInfo info = new SingletonBootInfo(gameObject, null, BootMode.BEFORE_SCENE_LOAD);

        //    // Act.
        //    bool condition = info.IsValid;

        //    // Assert.
        //    Assert.IsFalse(condition, "Expected a singleton boot info with a null singleton type to not be valid but it was.");

        //    // Cleanup
        //    GameObject.DestroyImmediate(gameObject);
        //}

        /// <summary>
        /// Tests whether the <see cref="SingletonBootInfo"/> can properly identify whether it is valid or not.
        /// It expects that if the singleton type is invalid, it returns as invalid.
        /// </summary>
        [Test]
        public void Test_IsValid_Invalid_SingletonType()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            SingletonBootInfo info = new SingletonBootInfo(gameObject, typeof(MonoBehaviour), BootModeType.DEFAULT);

            // Act.
            bool condition = info.IsValid;

            // Assert.
            Assert.IsFalse(condition, "Expected a singleton boot info with an invalid singleton type to not be valid but it was.");

            // Cleanup
            GameObject.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonBootInfo"/> can properly identify whether it is valid or not.
        /// It expects that if the arguments contain a valid gameobject, singleton type and boot mode, it will return as valid.
        /// </summary>
        [Test]
        public void Test_IsValid_True()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            SingletonBootInfo info = new SingletonBootInfo(gameObject, typeof(SingletonOne), BootModeType.DEFAULT);

            // Act.
            bool condition = info.IsValid;

            // Assert.
            Assert.IsTrue(condition, "Expected a singleton boot info with correct values to be valid but it wasn't.");

            // Cleanup
            GameObject.DestroyImmediate(gameObject);
        }
    }
}

#endif