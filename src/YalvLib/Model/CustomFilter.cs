using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YalvLib.Model
{
    public class CustomFilter
    {
        public CustomFilter()
        {
        }

        public CustomFilter(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; set; }
    }
}
