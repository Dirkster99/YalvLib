using YalvLib.Model;

namespace YalvLib.Providers
{
  using System;
  using System.Collections.Generic;
  using YalvLib.Domain;
  using YalvLib.ViewModel;

  /// <summary>
  /// Base class for file based log file providers
  /// </summary>
  public abstract class AbstractEntriesProvider
  {
    /// <summary>
    /// Minimum date available for this class and its inherating classes
    /// </summary>
    protected static readonly DateTime MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    /// <summary>
    /// Get collection of logitems representing a log file from this data source
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public IEnumerable<LogEntry> GetEntries(string dataSource)
    {
      return this.GetEntries(dataSource, new FilterParams());
    }

    /// <summary>
    /// Get collection of logitems filtered by <paramref name="filter"/>
    /// representing a log file from this data source
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public abstract IEnumerable<LogEntry> GetEntries(string dataSource, FilterParams filter);
  }
}