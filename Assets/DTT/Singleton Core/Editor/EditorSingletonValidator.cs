#if UNITY_EDITOR

using System;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// Provides helper functions for validating and retrieving singletons.
    /// </summary>
    public static class EditorSingletonValidator
    {
        /// <summary>
        /// The singleton type which concrete editor script implementations should derive from.
        /// </summary>
        private static readonly Type _editorScriptBase = typeof(LazyEditorSingletonBehaviour<>);

        /// <summary>
        /// The singleton type which concrete editor implementations should derive from.
        /// </summary>
        private static readonly Type _editorSingletonBaseType = typeof(EditorSingletonBehaviour<>);

        /// <summary>
        /// Returns whether a type is a subclass of the generic editor singleton behaviour class.
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>Whether the type is a subclass of the generic editor singleton behaviour class.</returns>
        public static bool IsSubClassOfEditorSingleton(Type toCheck) => SingletonValidator.IsSubClassOf(toCheck, _editorSingletonBaseType);

        /// <summary>
        /// Returns whether a type is a subclass of a generic script singleton behaviour.
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>Whether the type is a subclass of the generic script singleton behaviour class.</returns>
        public static bool IsSubClassOfScriptSingleton(Type toCheck) => SingletonValidator.IsSubClassOf(toCheck, _editorScriptBase);
    }
}

#endif
