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
        {
            _guidCounter = 0;
        }

        // This class actually manage only one workspace at a time (cf getter)
        // but still have a list of workspace that can be returned to be read

        private List<LogAnalysisWorkspace> _workspaces = new List<LogAnalysisWorkspace>();

        private LogAnalysisWorkspace _actualWorkSpace;

        private static YalvRegistry _singleton;

        private uint _guidCounter;

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
        public void SetActualLogAnalysisWorkspace(LogAnalysisWorkspace workspace)
        {
            if (_workspaces.Count == 1 && !_workspaces[0].LogEntries.Any())
            {
                _workspaces[0] = workspace;
            }

            if (!_workspaces.Contains(workspace))
                _workspaces.Add(workspace);
            
            ActualWorkspace = workspace;
        }

        /// <summary>
        /// return actual workspace or null if there is none
        /// </summary>
        public LogAnalysisWorkspace ActualWorkspace
        {
            get
            {
                if (LogAnalysisWorkspaces.Count == 0)
                    return null;
                return _actualWorkSpace;
            }
            set { _actualWorkSpace = value; }
        }


        public uint generateGuid()
        {
            return _guidCounter += 1;
        }

        /// <summary>
        /// Return a readonly collection containing the sessions
        /// </summary>
        public ReadOnlyCollection<LogAnalysisWorkspace> LogAnalysisWorkspaces
        {
            get
            {
                return new ReadOnlyCollection<LogAnalysisWorkspace>(_workspaces);
            }
        }
    }

}
