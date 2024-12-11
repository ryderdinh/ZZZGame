namespace DTT.Singletons.Exceptions
{
    /// <summary>
    /// Thrown when the singleton profile comes into conflict with the usage of singleton
    /// prefabs in the project.
    /// </summary>
    public class InvalidProfileSetupException : SingletonCoreException
    {
        #region Variables
        #region Private
        /// <summary>
        /// The prefixed message in front of any 
        /// <see cref="InvalidProfileSetupException"/> message.
        /// </summary>
        private const string PREFIX = "- [Invalid setup of profile was done] - ";
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SingletonCoreException"/> with given message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public InvalidProfileSetupException(string message) : base(Format(message)) { }
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

