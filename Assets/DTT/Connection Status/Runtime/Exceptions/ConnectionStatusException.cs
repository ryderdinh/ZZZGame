using System;

namespace DTT.Networking.ConnectionStatus.Exceptions
{
    /// <summary>
    /// The core exception class for connection status exceptions used in the package.
    /// </summary>
    public abstract class ConnectionStatusException : Exception
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any
        /// <see cref="ConnectionStatusException"/>
        /// </summary>
        private const string PREFIX = "[DTT] - [ConnectionStatusException] ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="ConnectionStatusException"/> with the given message
        /// to be preceded by the prefix.
        /// <param name="message">The message to show.</param>
        public ConnectionStatusException(string message) : base(Format(PREFIX, message)) { }
        #endregion

        #region Methods
        #region Protected
        /// <summary>
        /// Returns a formatted version of the given message using the <see cref="PREFIX"/>.
        /// </summary>
        /// <param name="prefix">The prefix value.</param>
        /// <param name="message">The message to be formatted.</param>
        /// <returns>The formatted message.</returns>
        protected static string Format(string prefix, string message) => message.Insert(0, prefix);
        #endregion
        #endregion
    }
}
