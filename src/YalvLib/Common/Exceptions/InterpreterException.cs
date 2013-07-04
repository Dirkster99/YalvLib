using System;

namespace YalvLib.Common.Exceptions
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message)
        {
        }
    }
}