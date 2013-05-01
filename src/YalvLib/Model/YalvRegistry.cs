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

        public void SetActualLogAnalysisSession(LogAnalysisSession session)
        {
            _sessions.Clear();
            _sessions.Add(session);
        }

        public LogAnalysisSession ActualSession
        {
            get
            {
                if (LogAnalysisSessions.Count == 0)
                    return null;
                return LogAnalysisSessions[0];
            }
        }

        public ReadOnlyCollection<LogAnalysisSession> LogAnalysisSessions
        {
            get
            {
                return new ReadOnlyCollection<LogAnalysisSession>(_sessions);
            }
        }
    }

}
