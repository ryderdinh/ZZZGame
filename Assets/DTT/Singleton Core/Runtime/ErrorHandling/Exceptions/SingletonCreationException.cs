namespace DTT.Singletons.Exceptions
{
    /// <summary>
    /// Thrown when a singleton instance could not be created.
    /// </summary>
    public class SingletonCreationException : SingletonCoreException
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any <see cref="SingletonCreationException"/>
        /// </summary>
        private const string PREFIX = "- [An error occured during singleton boot] - ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SingletonCreationException"/> with the given message
        /// to be preceded by the prefix.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public SingletonCreationException(string message) : base(Format(message)) { }
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

