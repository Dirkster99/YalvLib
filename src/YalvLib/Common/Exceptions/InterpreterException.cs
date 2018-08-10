namespace YalvLib.Common.Exceptions
{
    using System;

    /// <summary>
    /// Implements a simple exception based class that is typically
    /// thrown by an Interpreter implementation.
    /// </summary>
    public class InterpreterException : Exception
    {
        /// <summary>
        /// Class constructor from string message parameter which
        /// describes the interpretation exception in detail.
        /// </summary>
        /// <param name="message"></param>
        public InterpreterException(string message) : base(message)
        {
        }
    }
}