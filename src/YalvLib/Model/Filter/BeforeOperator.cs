namespace YalvLib.Model.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YalvLib.Common.Exceptions;

    /// <summary>
    /// Represents the BEFORE operator used on date expression comparison
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

        /// <summary>
        /// Evaluate if any date property in the list is before the value
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