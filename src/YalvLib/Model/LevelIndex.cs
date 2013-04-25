namespace YalvLib.Model
{
    /// <summary>
    /// Level index enumeration indicates the type of log file entry (warning, error, fatal error etc)
    /// </summary>
    public enum LevelIndex
    {
        /// <summary>
        /// Indicates an unknown type of log file entry
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Indicates a debug type of log file entry
        /// </summary>
        DEBUG = 1,

        /// <summary>
        /// Indicates an info type of log file entry
        /// </summary>
        INFO = 2,

        /// <summary>
        /// Indicates a warning type of log file entry
        /// </summary>
        WARN = 3,

        /// <summary>
        /// Indicates an error type of log file entry
        /// </summary>
        ERROR = 4,

        /// <summary>
        /// Indicates a fatal error type of log file entry
        /// </summary>
        FATAL = 5
    }
}