#if TEST_FRAMEWORK

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.Singletons.Tests
{
    /// <summary>
    /// Tests the singleton bootstrap process.
    /// </summary>
    public class Test_SingletonBoostrap
    {
        #region Variables
        /// <summary>
        /// The test singleton prefab with a child component that references the singleton component
        /// in its awake method.
        /// </summary>
        private GameObject _singletonWithChild;

        /// <summary>
        /// The test singleton prefab with a component that references the singleton component
        /// in its awake method before the singleton component can call its own awake.
        /// </summary>
        private GameObject _singletonWithParent;

        /// <summary>
        /// The default singleton prefabs to be created in succession.
        /// </summary>
        private Dictionary<Type, GameObject> _defaultSingletons;

        /// <summary>
        /// The singleton profile.
        /// </summary>
        private SingletonProfileSO _profile;
        #endregion

        #region Setup
        /// <summary>
        /// Loads relevant singleton data as to setup the tests.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _profile = Resources.Load<SingletonProfileSO>(SingletonConfig.PROFILE_LOAD_PATH);
            if (_profile == null)
            {
                Assert.Inconclusive("Profile could not be found but is necessary for the test to function.");
                return;
            }

            _singletonWithChild = Resources.Load<GameObject>("TestSingletonWithChild");
            _singletonWithParent = Resources.Load<GameObject>("TestSingletonWithParent");
            _defaultSingletons = new Dictionary<Type, GameObject>
            {
                { typeof(SingletonOne), Resources.Load<GameObject>("TestSingleton1") },
                { typeof(SingletonTwo), Resources.Load<GameObject>("TestSingleton2") },
                { typeof(SingletonThree), Resources.Load<GameObject>("TestSingleton3") },
            };
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests whether the bootstrap process handles the conflict of multiple singletons
        /// created in quick succession with them calling on each other in the Awake methods.
        /// It expects the Asserts in all test singleton components to succeed.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_BootConflict_Dependency_SingletonToSingleton()
        {
            _profile.Add(_defaultSingletons[typeof(SingletonOne)], typeof(SingletonOne), BootModeType.LAZY);
            _profile.Add(_defaultSingletons[typeof(SingletonTwo)], typeof(SingletonTwo), BootModeType.LAZY);
            _profile.Add(_defaultSingletons[typeof(SingletonThree)], typeof(SingletonThree), BootModeType.LAZY);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonOne>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonOne)]);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonTwo>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonTwo)]);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonThree>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonThree)]);

            yield return null;

            GameObject.Destroy(GameObject.FindObjectOfType<SingletonOne>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonTwo>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonThree>().gameObject);

            yield return null;

            CleanProfile();
        }

        /// <summary>
        /// Tests whether the bootstrap process handles the conflict of a singleton component
        /// being called upon by a component with the following conditions
        /// - The component is on the singleton prefab 
        /// - The singleton is called upon in the Awake or OnEnable method of the component.
        /// - The singleton is called upon after the Awake of the singleton has been called already.
        /// It expects the Asserts in all test singleton components to succeed.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_BootConflict_Dependency_SingletonToChildComponent()
        {
            _profile.Add(_singletonWithChild, typeof(SingletonOne), BootModeType.LAZY);
            _profile.Add(_defaultSingletons[typeof(SingletonTwo)], typeof(SingletonTwo), BootModeType.LAZY);
            _profile.Add(_defaultSingletons[typeof(SingletonThree)], typeof(SingletonThree), BootModeType.LAZY);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonOne>())
                GameObject.Instantiate(_singletonWithChild);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonTwo>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonTwo)]);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonThree>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonThree)]);

            yield return null;

            GameObject.Destroy(GameObject.FindObjectOfType<SingletonOne>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonTwo>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonThree>().gameObject);

            yield return null;

            CleanProfile();
        }

        /// <summary>
        /// Tests whether the bootstrap process handles the conflict of a singleton component
        /// being called upon by a component with the following conditions
        /// - The component is on the singleton prefab 
        /// - The singleton is called upon in the Awake or OnEnable method of the component.
        /// - The singleton is called upon before the Awake of the singleton has been called.
        /// It expects the Asserts in all test singleton components to succeed.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_BootConflict_Dependency_SingletonToParentComponent()
        {
            _profile.Add(_defaultSingletons[typeof(SingletonOne)], typeof(SingletonOne), BootModeType.LAZY);
            _profile.Add(_singletonWithParent, typeof(SingletonTwo), BootModeType.LAZY);
            _profile.Add(_defaultSingletons[typeof(SingletonThree)], typeof(SingletonThree), BootModeType.LAZY);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonOne>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonOne)]);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonTwo>())
                GameObject.Instantiate(_singletonWithParent);

            if (!SingletonBootstrapService.CheckSingletonExistence<SingletonThree>())
                GameObject.Instantiate(_defaultSingletons[typeof(SingletonThree)]);

            yield return null;

            GameObject.Destroy(GameObject.FindObjectOfType<SingletonOne>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonTwo>().gameObject);
            GameObject.Destroy(GameObject.FindObjectOfType<SingletonThree>().gameObject);

            yield return null;

            CleanProfile();
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Cleans up changes mdae to the profile.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup() => CleanProfile();
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Removes all test singleton prefabs from the profile.
        /// </summary>
        private void CleanProfile()
        {
            foreach (var singletonPair in _defaultSingletons)
                _profile.Remove(singletonPair.Value);

            _profile.Remove(_singletonWithChild);
            _profile.Remove(_singletonWithParent);
        }
        #endregion
        #endregion
    }
}
#endif