using System;

namespace TwinCAT
{
    /// <summary>
    /// EventArgs passed along with the event raised when a TwinCAT variable is
    /// updated.
    /// </summary>
    public class TwinCATVariableUpdatedEventArgs : EventArgs
    {
        /// <summary>Time when TwinCAT variable was updated.</summary>
        public readonly DateTime timestamp;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TwinCATVariableUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">When TwinCAT variable was updated.</param>
        public TwinCATVariableUpdatedEventArgs(DateTime timestamp)
        {
            this.timestamp = timestamp;
        }
    }
}
