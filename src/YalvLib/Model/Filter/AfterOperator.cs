using System;
using System.Collections.Generic;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
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
            if (property.GetType() != typeof (DateTime))
            {
                throw new InterpreterException(property + "Not a DateTime");
            }
            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == 1;
        }

        /// <summary>
        /// This function is never to be use for a date comparaison
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Evaluate(List<object> property, string value)
        {
            throw new NotImplementedException();
        }
    }
}