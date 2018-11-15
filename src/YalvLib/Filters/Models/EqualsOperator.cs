namespace Filters.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represent the equals operator
    /// </summary>
    public class EqualsOperator : Operator
    {
        /// <summary>
        /// Check if the given property value equals the given value
        /// </summary>
        /// <param name="property">Property value</param>
        /// <param name="value">Given value</param>
        /// <returns>true if equals, false otherwise</returns>
        public override bool Evaluate(object property, string value)
        {
            if (!(property is DateTime))
                return (property.ToString()).Equals(value,StringComparison.CurrentCultureIgnoreCase);

            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == 0;
        }

        /// <summary>
        /// Check if one of the given property values equals the given value
        /// </summary>
        /// <param name="properties">Property values</param>
        /// <param name="value">Given value</param>
        /// <returns>true if equals, false otherwise</returns>
        public override bool Evaluate(List<object> properties, string value)
        {
            return Enumerable.Contains(properties, value);
        }
    }
}