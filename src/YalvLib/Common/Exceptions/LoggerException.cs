using System;

namespace YalvLib.Common.Exceptions
{
    public class LoggerException : Exception
    {

        public LoggerException(string message, int line, int position):base(message)
        {
            Line = line;
            Position = position;
        }

        public int Line { get; set; }
        public int Position { get; set; }
    }
}