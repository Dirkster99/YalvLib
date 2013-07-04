using System;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
    public class BeforeOperator : Operator
    {
        public override bool Evaluate(object property, string value)
        {
            if (property.GetType() != typeof (DateTime))
            {
                throw new InterpreterException(property + " Not a DateTime");
            }
            return DateTime.Compare((DateTime) property, DateTime.Parse(value)) == -1;
        }
    }
}