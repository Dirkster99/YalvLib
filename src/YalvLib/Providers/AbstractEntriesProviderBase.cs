namespace YalvLib.Providers
{
    using System.Collections.Generic;
    using YalvLib.Domain;
    using YalvLib.Model;

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

        /// <summary>
        /// Implements a simple way for returning meaningful messages to the user.
        /// 
        /// Deriving classes should override and return their own user messages to
        /// give meaningful feedback to the user when reading was not possible without
        /// warnings or errors.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetMessages()
        {
            return new List<string>();
        }
    }
}