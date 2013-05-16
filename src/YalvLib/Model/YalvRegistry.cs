using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace YalvLib.Model
{

    public class YalvRegistry
    {

        private YalvRegistry()
        {}

        // This class actually manage only one workspace at a time (cf getter)
        // but still have a list of workspace that can be returned to be read

        private List<LogAnalysisWorkspace> _sessions = new List<LogAnalysisWorkspace>(); 

        private static YalvRegistry _singleton;

        public static YalvRegistry Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new YalvRegistry();
                return _singleton;
            }
        }

        /// <summary>
        /// Clears and replace the actual workspace 
        /// </summary>
        /// <param name="workspace"></param>
        public void SetActualLogAnalysisSession(LogAnalysisWorkspace workspace)
        {
            _sessions.Clear();
            _sessions.Add(workspace);
        }

        /// <summary>
        /// return actual workspace or null if there is none
        /// </summary>
        public LogAnalysisWorkspace ActualWorkspace
        {
            get
            {
                if (LogAnalysisSessions.Count == 0)
                    return null;
                return LogAnalysisSessions[0];
            }
        }

        /// <summary>
        /// Return a readonly collection containing the sessions
        /// </summary>
        public ReadOnlyCollection<LogAnalysisWorkspace> LogAnalysisSessions
        {
            get
            {
                return new ReadOnlyCollection<LogAnalysisWorkspace>(_sessions);
            }
        }
    }

}
