﻿namespace Filters.Models
{
    using log4netLib.Interfaces;
    using YalvLib.Model;

    /// <summary>
    /// Represent the context to be evaluate with
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Getter / setter of the entry
        /// </summary>
        public ILogEntry Entry { get; set; }

        /// <summary>
        /// Getter / setter of the Log Analysis
        /// </summary>
        public LogAnalysis Analysis { get; set; }
    }
}