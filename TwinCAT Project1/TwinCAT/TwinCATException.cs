using System;

namespace TwinCAT
{
    /// <summary>
    /// Exception thrown whenever an error occurs in the TwinCAT wrapper code.
    /// </summary>
    class TwinCATException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwinCATException"/>
        /// class.
        /// </summary>
        public TwinCATException(string message) : base(message)
        {
        }
    }
}
