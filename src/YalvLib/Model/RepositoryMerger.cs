namespace YalvLib.Model
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implements a merging functionality among a configured number of
    /// <see cref="LogEntryRepository"/> objects.
    /// </summary>
    public class RepositoryMerger
    {
        #region fields
        private readonly List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public RepositoryMerger()
        {
        }

        /// <summary>
        /// Class constructor from <see cref="LinkedList{LogEntryRepository}"/>
        /// </summary>
        public RepositoryMerger(List<LogEntryRepository> repos)
        {
            _sourceRepositories = repos;
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Add a source log entry repository into the list of repositories to be merged.
        /// </summary>
        /// <param name="sourceRepository"></param>
        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }

        /// <summary>
        /// Add a list of source log entry repositories into the list of repositories
        /// to be merged.
        /// </summary>
        /// <param name="sourceRepositories"></param>
        public void AddSourceRepositories(List<LogEntryRepository> sourceRepositories)
        {
            _sourceRepositories.AddRange(sourceRepositories);
        }

        /// <summary>
        /// Merge source repositories into one target repository.
        /// </summary>
        /// <returns>target repository is the result of merging previously
        /// configured source repositories</returns>
        public LogEntryRepository Merge()
        {
            IEnumerable<LogEntry> logEntries = GetLogEntries();
            IEnumerable<LogEntry> sortedLogEntries = logEntries.OrderBy(x => x.TimeStamp);

            var targetRepository = new LogEntryRepository();
            foreach (LogEntry logEntry in sortedLogEntries)
            {
                targetRepository.AddLogEntry(new LogEntry(logEntry));
            }

            return targetRepository;
        }

        private IEnumerable<LogEntry> GetLogEntries()
        {
            List<LogEntry> entries = new List<LogEntry>();
            foreach (LogEntryRepository repository in _sourceRepositories)
            {
                entries.AddRange(repository.LogEntries);
            }

            return entries;
        }
        #endregion methods
    }
}
