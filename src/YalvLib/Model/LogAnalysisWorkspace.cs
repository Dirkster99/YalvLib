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
        private IList<LogEntryRepository> _sourceRepositories = new List<LogEntryRepository>();
        private IList<LogAnalysis> _analyses = new List<LogAnalysis>();
        private LogAnalysis _currentAnalysis;
         

        /// <summary>
        /// Adds a sourceRepository to the source repositories of the actual LogAnalysisWorkspace
        /// </summary>
        /// <param name="sourceRepository"></param>
        public void AddSourceRepository(LogEntryRepository sourceRepository)
        {
            _sourceRepositories.Add(sourceRepository);
        }


        /// <summary>
        /// Add a list of repositories
        /// </summary>
        /// <param name="sourceRepositories">Repositories to add</param>
        public void AddSourceRepositories(List<LogEntryRepository> sourceRepositories)
        {
            foreach (var logEntryRepository in sourceRepositories)
            {
                _sourceRepositories.Add(logEntryRepository);
            }
        }



        /// <summary>
        /// Get the source reporitories list
        /// </summary>
        public IList<LogEntryRepository> SourceRepositories
        {
            get { return _sourceRepositories; }
            set { _sourceRepositories = value; }
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
        /// Get the source repositories list
        /// </summary>
        public IList<LogAnalysis> Analyses
        { get { return _analyses; } private set { _analyses = value; } }

        /// <summary>
        /// Getter / Setter of the current analysis
        /// </summary>
        public LogAnalysis CurrentAnalysis
        {
            get { return _currentAnalysis; }
            set { _currentAnalysis = value; if (!_analyses.Contains(value))_analyses.Add(value); }
        }

        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; protected set; }


        public override bool Equals(object obj)
        {
            var e = obj as LogAnalysisWorkspace;
            if (e == null)
                return false;
            return e.Uid == Uid;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
