using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace YalvLib.Model
{
    /// <summary>
    /// This class is a singleton representing the unique instance of the Registry
    /// containing the whole data
    /// </summary>
    public class YalvRegistry
    {
        // This class actually manage only one workspace at a time (cf getter)
        // but still have a list of workspace that can be returned to be read

        private static YalvRegistry _singleton;
        private readonly List<LogAnalysisWorkspace> _workspaces = new List<LogAnalysisWorkspace>();

        private LogAnalysisWorkspace _actualWorkSpace;

        private uint _guidCounter;

        private YalvRegistry()
        {
            _guidCounter = 0;
        }

        /// <summary>
        /// Return the instance of this class or create one if none exists
        /// </summary>
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


        /// <summary>
        /// Return a readonly collection containing the sessions
        /// </summary>
        public ReadOnlyCollection<LogAnalysisWorkspace> LogAnalysisWorkspaces
        {
            get { return new ReadOnlyCollection<LogAnalysisWorkspace>(_workspaces); }
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
        /// Counter for the Guid of the logentries
        /// </summary>
        /// <returns></returns>
        public uint GenerateGuid()
        {
            return _guidCounter += 1;
        }
    }
}