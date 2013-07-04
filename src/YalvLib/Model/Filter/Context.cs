namespace YalvLib.Model.Filter
{
    public class Context
    {
        private LogEntry _logEntry;
        private LogAnalysis _analysis;

        public LogEntry Entry
        {
            get { return _logEntry; }
            set { _logEntry = value; }
        }

        public LogAnalysis Analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }
    }
}