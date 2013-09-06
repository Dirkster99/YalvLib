using System;
using System.Collections.Generic;
using System.Linq;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent the operator BEFORE used on date expression comparaison
    /// </summary>
    public class BeforeOperator : Operator
    {
        /// <summary>
        /// Evaluate if date property is before the value
        /// </summary>
        /// <param name="property">property</param>
        /// <param name="value">value</param>
        /// <returns>true if before, false otherwise</returns>
        public override bool Evaluate(object property, string value)
        {
            if (property.GetType() != typeof(DateTime) && property.GetType() != typeof(String))
            {
                throw new InterpreterException(property + " Not a DateTime");
            }
            if (property is string)
            {
                return DateTime.Compare(DateTime.Parse(property.ToString()), DateTime.Parse(value)) == -1;
            }
            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == -1;
        }

        public override bool Evaluate(List<object> properties, string value)
        {
            return properties.Any(obj => Evaluate(obj, value));
        }
    }
}