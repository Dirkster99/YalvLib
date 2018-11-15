namespace Filters.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Filters.Exceptions;

    /// <summary>
    /// Represent the operator AFTER used on date expression comparaison
    /// </summary>
    public class AfterOperator : Operator
    {
        /// <summary>
        /// Evaluate if date property is after the value
        /// </summary>
        /// <param name="property">property</param>
        /// <param name="value">value</param>
        /// <returns>true if after, false otherwise</returns>
        public override bool Evaluate(object property, string value)
        {
            if (property.GetType() != typeof(DateTime) && property.GetType() != typeof(String))
            {
                throw new InterpreterException(property + " Not a DateTime");
            }
            if (property is string)
            {
                return DateTime.Compare(DateTime.Parse(property.ToString()), DateTime.Parse(value)) == 1;
            }
            return DateTime.Compare((DateTime)property, DateTime.Parse(value)) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Evaluate(List<object> properties, string value)
        {
            return properties.Any(obj => Evaluate(obj, value));
        }
    }
}