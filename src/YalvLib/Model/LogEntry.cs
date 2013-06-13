using System;

namespace YalvLib.Model
{
    /// <summary>
    /// Model an entry of a log file such that it can be consumed by a collection
    /// (to model all entries in a log file)
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        /// <summary>
        /// Empty constructor for the Fluent Nhibernate mapping
        /// </summary>
        public LogEntry()
        {
        }


        /// <summary>
        /// Constructor of a log entry
        /// </summary>
        /// <param name="entry">Entry to map</param>
        public LogEntry(LogEntry entry)
        {
            App = entry.App;
            TimeStamp = entry.TimeStamp;
            Logger = entry.Logger;
            Thread = entry.Thread;
            Message = entry.Message;
            Throwable = entry.Throwable;
            MachineName = entry.MachineName;
            UserName = entry.UserName;
            HostName = entry.HostName;
            Class = entry.Class;
            File = entry.File;
            Method = entry.Method;
            Line = entry.Line;
            LevelIndex = entry.LevelIndex;
        }

        #region properties

        /// <summary>
        /// Get/set Id of the log item relatively to its repository.
        /// </summary>
        public UInt32 Id { get; internal set; }

        /// <summary>
        /// Get/set Path of the file containing this log item
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Get/set Date and time of the moment in time when this log item was logged
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Get/set Time delta for this entry in comparison to the previous entry
        /// </summary>
        public string Delta { get; internal set; }

        /// <summary>
        /// Get/set logger name of log file
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// Get/set thread id that generated this log item.
        /// </summary>
        public string Thread { get; set; }

        /// <summary>
        /// Get/set message contained in this log item.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get/set name of machine on which this log item was created.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Get/set user name that was used to generate this log item.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Get/set hostname of computer on which log item was logged.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Get/set name of application that logged this item.
        /// </summary>
        public string App { get; set; }

        /// <summary>
        /// Get/set string representation of logged exception.
        /// </summary>
        public string Throwable { get; set; }

        /// <summary>
        /// Get/set class that logged this item.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Get/set method that logged this item.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Get/set file name that generated this entry
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Get/set line of code that logged this item.
        /// </summary>
        public UInt32 Line { get; set; }

        /// <summary>
        /// Get/set data from uncategorized column.
        /// </summary>
        public string Uncategorized { get; set; }

        /// <summary>
        /// Indicate kind of log4net message (Info, Warn, Debug, Error etc)
        /// </summary>
        public LevelIndex LevelIndex { get; set; }

        /// <summary>
        /// A unique identifier in the system.
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// a unique identifier in the workspace
        /// </summary>
        public uint GuId { get; set; }

        #endregion properties

        public override bool Equals(object obj)
        {
            var e = obj as LogEntry;
            if (e == null)
                return false;
            return e.App == App
                   && e.Class == Class
                   && e.Delta == Delta
                   && e.File == File
                   && e.HostName == HostName
                   && e.Line == Line
                   && e.TimeStamp == TimeStamp
                   && e.Logger == Logger
                   && e.MachineName == MachineName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}