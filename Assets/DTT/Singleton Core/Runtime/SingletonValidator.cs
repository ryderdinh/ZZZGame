using System;
using UnityEngine;

namespace DTT.Singletons
{
    /// <summary>
    /// Provides helper functions for validating and retrieving singletons.
    /// </summary>
    public static class SingletonValidator
    {
        #region Variables
        #region Public
        /// <summary>
        /// The singleton type which concrete implementations should derive from.
        /// </summary>
        public static readonly Type singletonBaseType = typeof(SingletonBehaviourBase<>);

        /// <summary>
        /// The singleton type which concrete lazy implementations should derive from.
        /// </summary>
        public static readonly Type runtimeScriptBase = typeof(LazySingletonBehaviour<>);

        /// <summary>
        /// The singleton type which concrete prefab implementations should derive from.
        /// </summary>
        public static readonly Type runtimePrefabBase = typeof(SingletonBehaviour<>);
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Returns whether given game could get one valid singleton component.
        /// </summary>
        /// <param name="gameObject">The prefab to check.</param>
        /// <param name="singletonType">The singleton type on the prefab.</param>
        /// <returns>Whether one valid singleton type has been found of the given game object.</returns>
        public static bool TryGetSingletonType(GameObject gameObject, out Type singletonType)
        {
            singletonType = null;

            if (gameObject == null)
                return false;

            Component[] components = gameObject.GetComponents(typeof(MonoBehaviour));
            if (components.Length == 0)
                return false;

            Type foundType = null;
            foreach (Component component in components)
            {
                // Take into account missing component reference.
                if (component != null)
                {
                    // Check if component is a subclass of the singleton.
                    Type type = component.GetType();
                    if (IsSubClassOfSingleton(type))
                    {
                        foundType = type;
                        // Only set the found type the first time taking into account duplicate singleton components.
                        if (singletonType == null)
                            singletonType = foundType;
                        else
                            return false;
                    }
                }
            }

            singletonType = foundType;
            return foundType != null;
        }

        /// <summary>
        /// Returns whether a game object holds a singleton type component.
        /// </summary>
        /// <param name="gameObject">The game object to check.</param>
        /// <returns>Whether it holds a singleton type component.</returns>
        public static bool HasSingletonComponent(GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            Component[] components = gameObject.GetComponents(typeof(MonoBehaviour));
            if (components.Length == 0)
                return false;

            foreach (Component component in components)
            {
                // Take into account missing component reference.
                if (component != null)
                {
                    // Check if component is a subclass of the singleton.
                    if (IsSubClassOfSingleton(component.GetType()))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether given type is a subclass of the generic singleton behaviour class.
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>Whether the given type derives from the _singletonType type definition.</returns>
        public static bool IsSubClassOfSingleton(Type toCheck) => IsSubClassOf(toCheck, singletonBaseType);

        /// <summary>
        /// Returns whether a type is a subclass of a generic script singleton behaviour.
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>Whether the type is a subclass of the generic script singleton behaviour class.</returns>
        public static bool IsSubClassOfScriptSingleton(Type toCheck) => IsSubClassOf(toCheck, runtimeScriptBase);

        /// <summary>
        /// Returns whether a type is a subclass of a generic prefab singleton behaviour.
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>Whether the type is a subclass of the generic prefab singleton behaviour class.</returns>
        public static bool IsSubClassOfPrefabSingleton(Type toCheck) => IsSubClassOf(toCheck, runtimePrefabBase);

        /// <summary>
        /// Returns whether a type derives from another type.
        /// <para>Include generic definition checks.</para>
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <param name="baseType">The base type.</param>
        /// <returns>Whether the type derives from the base type.</returns>
        public static bool IsSubClassOf(Type toCheck, Type baseType)
        {
            // Recursively go through up the inheritance tree to
            // check if type is a subclass of a base type.
            while (toCheck != null && toCheck != typeof(object))
            {
                var type = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == type)
                    return true;

                toCheck = toCheck.BaseType;
            }
            return false;
        }
        #endregion
        #endregion
    }
}