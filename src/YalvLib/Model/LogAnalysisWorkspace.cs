using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YalvLib.Model
{
    /// <summary>
    /// The LogAnalysisWorkspace contains every reporitories of LogEntries, ie all the log entries 
    /// are to be findable in an instance of LogAnalysisWorkspace
    /// </summary>
    public class LogAnalysisWorkspace
    {
        private readonly List<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();
        /*
         * We will be able to implement multiple LogAnalysis, but for the moment
         * we will working on one only.
        private readonly List<LogAnalysis> _analysis = new List<LogAnalysis>(); 
         */

        private LogAnalysis _analysis = new LogAnalysis();

        /// <summary>
        /// Adds a sourceRepository to the source repositories of the actual LogAnalysisWorkspace
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
        /// Get the source reporitories list
        /// </summary>
        public LogAnalysis Analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }


        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; protected set; }

    }
}
