using YalvLib.Model;

namespace YalvLib.Providers
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;

  using YalvLib.Domain;
  using YalvLib.ViewModel;

  /// <summary>
  /// Base class for database data providers.
  /// </summary>
  public abstract class AbstractEntriesProviderBase
  {

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