#if TEST_FRAMEWORK

using NUnit.Framework;
using System;
using UnityEngine;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the <see cref="SingletonValidator"/>.
    /// </summary>
    public class Test_SingletonValidator
    {
        #region InnerClasses
        /// <summary>
        /// A testable implementation of a singleton behaviour.
        /// </summary>
        private class TestableSingleton : SingletonBehaviour<TestableSingleton> { }

        /// <summary>
        /// Another testable implementation of a singleton behaviour.
        /// </summary>
        private class AnotherTestableSingleton : SingletonBehaviour<AnotherTestableSingleton> { }

        /// <summary>
        /// A testable singleton that derives from the wrong base class.
        /// </summary>
        private class BadlyDerivedSingleton : MonoBehaviour { }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can extract a singleton type
        /// from a prefab that is null. It expects that the return boolean value of this operation
        /// returns false and the output returns a null reference. 
        /// </summary>
        [Test]
        public void Test_TryGetSingletonType_Null()
        {
            // Call method with singleton being null.
            bool nullSingletonValue = SingletonValidator.TryGetSingletonType(null, out Type nullArg);

            // Null singleton value should be false since the game object is null and the output
            // value should be null because of this.
            Assert.IsFalse(nullSingletonValue, "One singleton type was found correctly while " +
                "the game object was null");
            Assert.IsNull(nullArg, "A singleton type was outputted while the game object " +
                "was null");
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can extract a singleton type
        /// from a prefab that has no component. It expects that the return boolean value of this operation
        /// returns false and the output returns a null reference. 
        /// </summary>
        [Test]
        public void Test_TryGetSingletonType_NoComponents()
        {
            // Create singleton with testable singleton component attached.
            const string name = "test-singleton";
            GameObject singleton = new GameObject(name);

            // Call method with gameobject having no components.
            bool noComponentsSingletonValue =
                SingletonValidator.TryGetSingletonType(singleton, out Type noComponentArg);

            // NoComponentsSingletonValue should be false since the game object doesn't have any components
            // and the output value should be null because of this.
            Assert.IsFalse(noComponentsSingletonValue, "One singleton type was found while the" +
                "game object didn't have any components");
            Assert.IsNull(noComponentArg, "A singleton type was outputted while the game object" +
                "didnt have any components");

            //clean up the singleton
            GameObject.DestroyImmediate(singleton);
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can extract a singleton type
        /// from a prefab that has singleton component(s) attached. It expects that the return
        /// boolean value of this operation returns true and the output returns the type when 
        /// one component is attached and false and the a type when there are multiple.
        /// </summary>
        [Test]
        public void Test_TryGetSingletonType_ComponentChecks()
        {
            // Create singleton with testable singleton component attached.
            const string name = "test-singleton";
            GameObject singleton = new GameObject(name);
            singleton.AddComponent<TestableSingleton>();

            // Call method with gameobject having no components.
            bool singletonComponentValue =
                SingletonValidator.TryGetSingletonType(singleton, out Type singletonComponentArg);

            // SingletonComponentValue should be true since the game object has a singleton component 
            // and the output value should not be null because of this.
            Assert.IsTrue(singletonComponentValue, "No singleton type was found while there" +
                "was one attached to given game object");
            Assert.IsNotNull(singletonComponentArg, "No singleton type was outputted while" +
                "there was one attached to given game gameobject");

            singleton.AddComponent<AnotherTestableSingleton>();
            singletonComponentArg = null;

            bool doubleSingletonComponentValue =
                SingletonValidator.TryGetSingletonType(singleton, out singletonComponentArg);

            // DoubleSingletonComponentValue should be false since the game object 
            // has 2 singleton components and the output value should not be null because of this.
            Assert.IsFalse(doubleSingletonComponentValue, "One singleton type was found while" +
                "there where 2 attached to the given game object");
            Assert.IsNotNull(singletonComponentArg, "No singleton type was found while there" +
                "were 2 attached to the given game object");

            // Clean up the singleton.
            GameObject.DestroyImmediate(singleton);
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can extract a singleton type
        /// from a prefab that has singleton component(s) attached. It expects that this operation
        /// returns false and the output returns true when the game object has a component that derives 
        /// from the wrong base type
        /// </summary>
        [Test]
        public void Test_TryGetSingletonType_InheritanceCheck()
        {
            // Create singleton with testable singleton component attached.
            const string name = "test-singleton";
            GameObject singleton = new GameObject(name);
            singleton.AddComponent<BadlyDerivedSingleton>();

            // Call method with gameobject having no components.
            bool badlyDerivedSingletonValue =
                SingletonValidator.TryGetSingletonType(singleton, out Type badlyDerivedSingletonArg);

            // BadlyDerivedSingletonValue should be false since the game object has a component 
            // that implements the wrong base class and the output value should be null because 
            // of this.
            Assert.IsFalse(badlyDerivedSingletonValue, "One singleton type was found while" +
                "no valid singleton type was attached to the given game object");
            Assert.IsNull(badlyDerivedSingletonArg, "A singleton type was outputted while" +
                "no valid singleton type was attached to the given game object");

            // Clean up the singleton.
            GameObject.DestroyImmediate(singleton);
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can detect whether a type is a subclass
        /// of DTT's abstract <see cref="SingletonBehaviour{T}"/> type. It expects that this operation 
        /// returns false on null references.
        /// </summary>
        [Test]
        public void Test_IsSubclassOfSingleton_Null()
        {
            // Arange.
            Type type = null;

            // Act.
            bool condition = SingletonValidator.IsSubClassOfSingleton(type);

            // Assert.
            Assert.IsFalse(condition, "Expected a null type not to be a sub class of a singleton but it was.");
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can detect whether a type is a subclass
        /// of DTT's abstract <see cref="SingletonBehaviour{T}"/> type. It expects that this operation 
        /// returns false on wrong types.
        /// </summary>
        [Test]
        public void Test_IsSubclassOfSingleton_WrongType()
        {
            // Arrange.
            Type type = typeof(MonoBehaviour);

            // Act.
            bool condition = SingletonValidator.IsSubClassOfSingleton(type);

            // Assert.
            Assert.IsFalse(condition, "A monobehaviour was returned as a subclass of a singleton.");
        }

        /// <summary>
        /// Tests whether the <see cref="SingletonValidator"/> can detect whether a type is a subclass
        /// of DTT's abstract <see cref="SingletonBehaviour{T}"/> type. It expects that this operation 
        /// returns true on a valid concrete implementation.
        /// </summary>
        [Test]
        public void Test_IsSubclassOfSingleton_CorrectType()
        {
            // Arrange.
            Type type = typeof(TestableSingleton);

            // Act.
            bool condition = SingletonValidator.IsSubClassOfSingleton(type);

            // Assert.         
            Assert.IsTrue(condition, "The testable singleton that is a valid singleton " +
                                            "was not returned as a subclass of a singleton");
        }
        #endregion
    }

}

#endif