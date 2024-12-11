using System;

namespace DTT.Singletons.Exceptions
{
    /// <summary>
    /// The core exception class for exceptions used in the singleton core package.
    /// </summary>
    public abstract class SingletonCoreException : Exception
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message before any <see cref="SingletonCoreException"/> message.
        /// </summary>
        private const string PREFIX = "[DTT] - [SingletonCore] ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SingletonCoreException"/> with given message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public SingletonCoreException(string message) : base(Format(message)) { }
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Returns a formatted version of the given message using the <see cref="PREFIX"/>.
        /// </summary>
        /// <param name="message">The message to be formatted.</param>
        /// <returns></returns>
        private static string Format(string message) => message.Insert(0, PREFIX);
        #endregion
        #endregion
    }
}

