using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YalvLib.Model
{
    /// <summary>
    /// This class represent a filter query
    /// </summary>
    public class CustomFilter
    {
        /// <summary>
        /// Empty constructor for mapping
        /// </summary>
        public CustomFilter()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">filter query as a string</param>
        public CustomFilter(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Getter / Setter filter query
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; set; }
    }
}
