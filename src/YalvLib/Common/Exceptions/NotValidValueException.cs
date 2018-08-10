namespace YalvLib.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implements a simple exception class that indicates
    /// a wrong parameter value usage when being thrown.
    /// </summary>
    public class NotValidValueException : Exception
    {
        /// <summary>
        /// Default class constructor.
        /// </summary>
        public NotValidValueException()
        {
        }

        /// <summary>
        /// Class constructor from string message parameter which
        /// describes the exception in detail.
        /// </summary>
        /// <param name="message"></param>
        public NotValidValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Class constructor from string message parameter and
        /// inner exception to describe the thrown exception in detail.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public NotValidValueException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Protected class constructor from SerializationInfo and StreamingContext
        /// to describe the context of the exception thrown in detail.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NotValidValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}