#if UNITY_EDITOR

namespace DTT.Singletons.Exceptions.Editor
{
    /// <summary>
    /// Thrown when an error has occured during a singleton import operation.
    /// </summary>
    public class SingletonImportException : SingletonCoreException
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any 
        /// <see cref="SingletonImportException"/> message.
        /// </summary>
        private const string PREFIX = "- [An error occured during singleton import] - ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SingletonCoreException"/> with given message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public SingletonImportException(string message) : base(Format(message)) { }
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Returns a formatted version of the given message using the <see cref="PREFIX"/>.
        /// </summary>
        /// <param name="message">The message to be formatted.</param>
        /// <returns>The formatted message</returns>
        private static string Format(string message) => message.Insert(0, PREFIX);
        #endregion
        #endregion
    }
}

#endif
