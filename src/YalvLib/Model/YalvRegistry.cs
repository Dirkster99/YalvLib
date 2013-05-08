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

        // This class actually manage only one session at a time (cf getter)
        // but still have a list of session that can be returned to be read

        private List<LogAnalysisSession> _sessions = new List<LogAnalysisSession>(); 

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
        /// Clears and replace the actual session 
        /// </summary>
        /// <param name="session"></param>
        public void SetActualLogAnalysisSession(LogAnalysisSession session)
        {
            _sessions.Clear();
            _sessions.Add(session);
        }

        /// <summary>
        /// return actual session or null if there is none
        /// </summary>
        public LogAnalysisSession ActualSession
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
        public ReadOnlyCollection<LogAnalysisSession> LogAnalysisSessions
        {
            get
            {
                return new ReadOnlyCollection<LogAnalysisSession>(_sessions);
            }
        }
    }

}
