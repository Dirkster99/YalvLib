using YalvLib.Providers;

namespace YalvLib.Model
{
    /// <summary>
    /// This class represent a repository based on a sql file
    /// </summary>
    public class LogEntrySqliteRepository : LogEntryRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path of the sql file</param>
        public LogEntrySqliteRepository(string path)
        {
            Path = path;
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider(EntriesProviderType.Sqlite);
            AddLogEntries(provider.GetEntries(path));
        }
    }
}