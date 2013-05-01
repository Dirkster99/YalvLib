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

        private List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();

        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }

        public IList<LogEntryRepository> SourceRepositories
        {
            //            get { return new ReadOnlyCollection<LogEntryRepository>(_sourceRepositories); }
            get { return _sourceRepositories; }
            protected set {}
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

        public Guid Uid { get; protected set; }
    }

}
