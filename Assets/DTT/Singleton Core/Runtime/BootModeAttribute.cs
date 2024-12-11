using System;

namespace DTT.Singletons
{
    /// <summary>
    /// An attribute used to define a prefered bootmode for a singleton that
    /// derives from the <see cref="SingletonBehaviour{T}"/> class.
    /// </summary>
    public class BootModeAttribute : Attribute
    {
        /// <summary>
        /// The prefered boot type.
        /// </summary>
        public readonly BootModeType bootType;

        /// <summary>
        /// Stores the prefered boot type.
        /// </summary>
        /// <param name="bootType">The prefered boot type.</param>
        public BootModeAttribute(BootModeType bootType) => this.bootType = bootType;
    }
}

