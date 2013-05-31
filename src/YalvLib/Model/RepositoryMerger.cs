using System.Collections.Generic;
using System.Linq;

namespace YalvLib.Model
{
    public class RepositoryMerger
    {
        private readonly List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();

        public RepositoryMerger()
        {
        }


        public RepositoryMerger(List<LogEntryRepository> repos)
        {
            _sourceRepositories = repos;
        }

        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }

        public void AddSourceRepositories(List<LogEntryRepository> sourceRepositories)
        {
            _sourceRepositories.AddRange(sourceRepositories);
        }

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
            var session = new LogAnalysisWorkspace();
            foreach (LogEntryRepository repository in _sourceRepositories)
            {
                session.AddSourceRepository(repository);
            }
            IEnumerable<LogEntry> logEntries = session.LogEntries;
            return logEntries;
        }
    }
}
