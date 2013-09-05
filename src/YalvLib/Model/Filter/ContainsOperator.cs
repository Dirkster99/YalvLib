using System;
using System.Collections.Generic;
using System.Linq;

namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent the operator contains
    /// </summary>
    public class ContainsOperator : Operator
    {
        /// <summary>
        /// Evalute the given property, checking if the given value is contained in the property value
        /// </summary>
        /// <param name="property">Property value</param>
        /// <param name="value">Given value</param>
        /// <returns>true if the value is contained in the property value, false otherwise</returns>
        public override bool Evaluate(object property, string value)
        {
            if (property is string)
                return ((string) property).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
            if (property.GetType() != typeof (DateTime))
                return property.ToString().Contains(value);
            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == 0;
        }

        /// <summary>
        /// Evalute the list of properties, checking if the given value is contained in one of the given property value
        /// </summary>
        /// <param name="properties">List of property values</param>
        /// <param name="value">given value</param>
        /// <returns>true if the value is contained in one of the property values, false otherwise</returns>
        public override bool Evaluate(List<object> properties, string value)
        {
            return properties.Any(obj => Evaluate(obj, value));
        }
    }
}