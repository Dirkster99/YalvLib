namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent the context to be evaluate with
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Getter / setter of the entry
        /// </summary>
        public LogEntry Entry { get; set; }

        /// <summary>
        /// Getter / setter of the Log Analysis
        /// </summary>
        public LogAnalysis Analysis { get; set; }
    }
}