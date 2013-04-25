using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.ViewModel;

namespace YalvLib.Model
{

    public class LogAnalysisSession
    {
        private List<LogEntryFileRepository> _sourceRepositories = new List<LogEntryFileRepository>();

        public void AddSourceRepository(LogEntryFileRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }

        public ReadOnlyCollection<LogEntryFileRepository> SourceRepositories
        {
            get { return new ReadOnlyCollection<LogEntryFileRepository>(_sourceRepositories); }
        }

        public ReadOnlyCollection<LogEntry> LogEntries
        {
            get
            {
                List<LogEntry> entries = new List<LogEntry>();
                foreach (var repository in SourceRepositories)
                {
                    entries.AddRange(repository.LogEntries);
                }
                return new ReadOnlyCollection<LogEntry>(entries);
            }
        }
    }

}
