
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
        private string mLevel;

        #region properties
        /// <summary>
        /// Get/set Id of the log item
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Get/set Path of the file containing this log item
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Get/set Date and time of the moment in time when this log item was logged
        /// </summary>
        public DateTime TimeStamp { get; internal set; }

        /// <summary>
        /// Get/set Time delta for this entry in comparison to the previous entry
        /// </summary>
        public string Delta { get; internal set; }

        /// <summary>
        /// Get/set logger name of log file
        /// </summary>
        public string Logger { get; internal set; }

        /// <summary>
        /// Get/set thread id that generated this log item.
        /// </summary>
        public string Thread { get; internal set; }

        /// <summary>
        /// Get/set message contained in this log item.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Get/set name of machine on which this log item was created.
        /// </summary>
        public string MachineName { get; internal set; }

        /// <summary>
        /// Get/set user name that was used to generate this log item.
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// Get/set hostname of computer on which log item was logged.
        /// </summary>
        public string HostName { get; internal set; }

        /// <summary>
        /// Get/set name of application that logged this item.
        /// </summary>
        public string App { get; internal set; }

        /// <summary>
        /// Get/set string representation of logged exception.
        /// </summary>
        public string Throwable { get; internal set; }

        /// <summary>
        /// Get/set class that logged this item.
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// Get/set method that logged this item.
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// Get/set file name that generated this entry
        /// </summary>
        public string File { get; internal set; }

        /// <summary>
        /// Get/set line of code that logged this item.
        /// </summary>
        public string Line { get; internal set; }

        /// <summary>
        /// Get/set data from uncategorized column.
        /// </summary>
        public string Uncategorized { get; internal set; }

        /// <summary>
        /// Indicate kind of log4net message (Info, Warn, Debug, Error etc)
        /// </summary>
        public LevelIndex LevelIndex { get; internal set; }

        /// <summary>
        /// Get/set kind of log4net message (Info, Warn, Debug, Error etc) as string
        /// </summary>
        public string Level
        {
            get
            {
                return this.mLevel;
            }
            set
            {
                if (value != this.mLevel)
                {
                    this.mLevel = value;
                    this.AssignLevelIndex(this.mLevel);
                }
            }
        }


        #endregion properties

        #region private methods
        private void AssignLevelIndex(String level)
        {
            String ul = !string.IsNullOrWhiteSpace(level) ? level.Trim().ToUpper() : String.Empty;
            LevelIndex levelIndexParsed;
            try
            {
                Enum.TryParse(ul, true, out levelIndexParsed);                
            } catch
            {
                levelIndexParsed = LevelIndex.NONE;
            }
            LevelIndex = levelIndexParsed;
        }
        #endregion private methods
    }
}
