using System;

namespace YalvLib.Model.Filter
{
    public class EqualsOperator : Operator
    {
        public override bool Evaluate(object property, string value)
        {
            if (property.GetType() != typeof (DateTime))
                return property.Equals(value);
            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == 0;
        }
    }
}