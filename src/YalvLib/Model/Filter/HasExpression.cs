using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
    public class HasExpression : BooleanExpression
    {


        public HasExpression(string operatorName, string propertyName)
        {
            if (operatorName.IndexOf("has", StringComparison.OrdinalIgnoreCase) < 0)
                throw new InterpreterException("Wrong operator, use the HAS operator please");
            if (propertyName.IndexOf("textmarker", StringComparison.OrdinalIgnoreCase) < 0)
                throw new InterpreterException("Wrong Property, use the TextMarker operator please");
        }

        public override bool Evaluate(Context context)
        {
            return context.Analysis.GetTextMarkersForEntry(context.Entry).Count > 0;
        }
    }
}
