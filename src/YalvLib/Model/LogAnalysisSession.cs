using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YalvLib.Model
{
    /// <summary>
    /// The LogAnalysisSession contains every reporitories of LogEntries, ie all the log entries 
    /// are to be findable in an instance of LogAnalysisSession
    /// </summary>
    public class LogAnalysisSession
    {
        private readonly List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();

        /// <summary>
        /// Adds a sourceRepository to the source repositories of the actual LogAnalysisSession
        /// </summary>
        /// <param name="sourceRepository"></param>
        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }


        /// <summary>
        /// Get the source reporitories list
        /// </summary>
        public IList<LogEntryRepository> SourceRepositories
        {
            get { return _sourceRepositories; }
        }


        /// <summary>
        /// Get all the entries of all the source repositories
        /// </summary>
        public ReadOnlyCollection<LogEntry> LogEntries
        {
            get
            {
                var entries = new List<LogEntry>();
                foreach (LogEntryRepository repository in SourceRepositories)
                {
                    entries.AddRange(repository.LogEntries);
                }
                return new ReadOnlyCollection<LogEntry>(entries);
            }
        }


        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; protected set; }
    }
}
