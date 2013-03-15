namespace YalvLib.Domain
{
  using System;

  public enum LevelIndex
  {
    NONE = 0,
    DEBUG = 1,
    INFO = 2,
    WARN = 3,
    ERROR = 4,
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
    public int Id { get; set; }

    public string Path { get; set; }

    public DateTime TimeStamp { get; set; }

    public string Delta { get; set; }

    public string Logger { get; set; }

    public string Thread { get; set; }

    public string Message { get; set; }

    public string MachineName { get; set; }

    public string UserName { get; set; }

    public string HostName { get; set; }

    public string App { get; set; }

    public string Throwable { get; set; }

    public string Class { get; set; }

    public string Method { get; set; }

    public string File { get; set; }

    public string Line { get; set; }

    public string Uncategorized { get; set; }

    /// <summary>
    /// Indicate kind of log4net message (Info, Warn, Debug, Error etc)
    /// </summary>
    public LevelIndex LevelIndex { get; set; }

    /// <summary>
    /// Level Property
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
