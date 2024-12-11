namespace DTT.Singletons.Exceptions
{
    /// <summary>
    /// Thrown when an error occured when a singleton instance is accessed.
    /// </summary>
    public class SingletonInstanceException : SingletonCoreException
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any <see cref="SingletonInstanceException"/>
        /// </summary>
        private const string PREFIX = "- [An error occured when a singleton instance was accessed] - ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SingletonInstanceException"/> with the given message
        /// to be preceded by the prefix.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public SingletonInstanceException(string message) : base(Format(message)) { }
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Returns a formatted version of the given message using the <see cref="PREFIX"/>.
        /// </summary>
        /// <param name="message">The message to be formatted.</param>
        /// <returns>The formatted message.</returns>
        private static string Format(string message) => message.Insert(0, PREFIX);
        #endregion
        #endregion
    }
}


