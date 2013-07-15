using System.Collections.Generic;

namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Abstract top class of an operator
    /// </summary>
    public abstract class Operator
    {
        public abstract bool Evaluate(object property, string value);
        public abstract bool Evaluate(List<object> properties, string value);
    }
}