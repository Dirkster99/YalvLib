namespace Filters.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Abstract top class of an operator
    /// </summary>
    public abstract class Operator
    {
        /// <summary>
        /// Evaluate if property has a certain relationship to the value
        /// The actual relationship/evaluation is determined in the inheriting class.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool Evaluate(object property, string value);

        /// <summary>
        /// Evaluate if any property in the list has a certain relationship to the value
        /// The actual relationship/evaluation is determined in the inheriting class.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool Evaluate(List<object> properties, string value);
    }
}