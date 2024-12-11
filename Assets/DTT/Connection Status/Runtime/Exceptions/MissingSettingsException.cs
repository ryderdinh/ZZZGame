namespace DTT.Networking.ConnectionStatus.Exceptions
{
    /// <summary>
    /// Thrown when settings used for handling the connection status are missing.
    /// </summary>
    public class MissingSettingsException : ConnectionStatusException
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any
        /// <see cref="MissingSettingsException"/>
        /// </summary>
        private const string PREFIX = "- [Encountered missing settings] - ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="MissingSettingsException"/> with the given message
        /// to be preceded by the prefix.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public MissingSettingsException(string message) : base(Format(PREFIX, message)) { }
        #endregion
    }
}
