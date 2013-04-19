namespace YalvLib.ViewModel
{
  using System;

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

  /// <summary>
  /// Model an entry of a log file such that it can be consumed by a collection
  /// (to model all entries in a log file)
  /// </summary>
  [Serializable]
  public class LogItem
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

      internal set
      {
        if (value != this.mLevel)
        {
          this.mLevel = value;
          this.assignLevelIndex(this.mLevel);
        }
      }
    }
    #endregion properties

    #region private methods
    private void assignLevelIndex(string level)
    {
      string ul = !string.IsNullOrWhiteSpace(level) ? level.Trim().ToUpper() : string.Empty;

      switch (ul)
      {
        case "DEBUG":
          LevelIndex = LevelIndex.DEBUG;
          break;

        case "INFO":
          LevelIndex = LevelIndex.INFO;
          break;

        case "WARN":
          LevelIndex = LevelIndex.WARN;
          break;

        case "ERROR":
          LevelIndex = LevelIndex.ERROR;
          break;

        case "FATAL":
          LevelIndex = LevelIndex.FATAL;
          break;

        default:
          LevelIndex = LevelIndex.NONE;
          break;
      }
    }
    #endregion private methods
  }
}
