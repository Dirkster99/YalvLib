namespace log4netLib.Interfaces
{
    using log4netLib.Enums;
    using System;

    /// <summary>
    /// Model an entry of a log file such that it can be consumed by a collection
    /// (to model all entries in a log file)
    /// </summary>
    public interface ILogEntry
    {
        #region properties
        /// <summary>
        /// Get/set Id of the log item relatively to its repository.
        /// </summary>
        UInt32 Id { get; }

        /// <summary>
        /// Get/set Path of the file containing this log item
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Get/set Date and time of the moment in time when this log item was logged
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Get/set Time delta for this entry in comparison to the previous entry
        /// </summary>
        string Delta { get; }

        /// <summary>
        /// Get/set logger name of log file
        /// </summary>
        string Logger { get; set; }

        /// <summary>
        /// Get/set thread id that generated this log item.
        /// </summary>
        string Thread { get; set; }

        /// <summary>
        /// Get/set message contained in this log item.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Get/set name of machine on which this log item was created.
        /// </summary>
        string MachineName { get; set; }

        /// <summary>
        /// Get/set user name that was used to generate this log item.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Get/set hostname of computer on which log item was logged.
        /// </summary>
        string HostName { get; set; }

        /// <summary>
        /// Get/set name of application that logged this item.
        /// </summary>
        string App { get; set; }

        /// <summary>
        /// Get/set string representation of logged exception.
        /// </summary>
        string Throwable { get; set; }

        /// <summary>
        /// Get/set class that logged this item.
        /// </summary>
        string Class { get; set; }

        /// <summary>
        /// Get/set method that logged this item.
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// Get/set file name that generated this entry
        /// </summary>
        string File { get; set; }

        /// <summary>
        /// Get/set line of code that logged this item.
        /// </summary>
        UInt32 Line { get; set; }

        /// <summary>
        /// Get/set data from uncategorized column.
        /// </summary>
        string Uncategorized { get; set; }

        /// <summary>
        /// Indicate kind of log4net message (Info, Warn, Debug, Error etc)
        /// </summary>
        LevelIndex LevelIndex { get; set; }

        /// <summary>
        /// A unique identifier in the system.
        /// </summary>
        Guid Uid { get; set; }

        /// <summary>
        /// a unique identifier in the workspace
        /// </summary>
        uint GuId { get; set; }
        #endregion properties
    }
}