namespace YalvLib.Domain
{
    using System;

    /// <summary>
    /// Model to store the possible parameters of an applicable filter.
    /// </summary>
    public class FilterParams
    {
        /// <summary>
        /// Gets/sets whether entries are filtered by this date.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets/sets whether entries are filtered by this log level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets/sets whether entries are filtered by this thread id.
        /// </summary>
        public string Thread { get; set; }

        /// <summary>
        /// Gets/sets whether entries are filtered by this particular logger.
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// Gets/sets whether entries are filtered by this particular message.
        /// </summary>
        public string Message { get; set; }

////        public string Pattern { get; set; }

        /// <summary>
        /// Standard method to improve debuging or raw data display for this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "Date: {0}, Level: {1}, Thread: {2}, Logger: {3}, Message: {4}",
                Date, Level, Thread, Logger, Message);
        }
    }
}