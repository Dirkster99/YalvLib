using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YalvLib.Model
{

    public class RepositoryMerger
    {

        private List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>(); 

        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }

        public LogEntryRepository Merge()
        {
            var logEntries = GetLogEntries();
            IEnumerable<LogEntry> sortedLogEntries = logEntries.OrderBy(x => x.TimeStamp);
            LogEntryRepository targetRepository = new LogEntryRepository();
            foreach (var logEntry in sortedLogEntries)
            {
                targetRepository.AddLogEntry(new LogEntry(logEntry));
            }
            return targetRepository;
        }

        private IEnumerable<LogEntry> GetLogEntries()
        {
            LogAnalysisSession session = new LogAnalysisSession();
            foreach (var repository in _sourceRepositories)
            {
                session.AddSourceRepository(repository);
            }
            IEnumerable<LogEntry> logEntries = session.LogEntries;
            return logEntries;
        }
    }

}
