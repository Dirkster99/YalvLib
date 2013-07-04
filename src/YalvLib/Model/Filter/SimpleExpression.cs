using System;
using System.Reflection;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
    public class SimpleExpression : BooleanExpression
    {
        private readonly Not _not;
        private readonly Operator _operator;
        private readonly string _propertyName;
        private readonly string _propertyValue;
        private PropertyInfo _propertyInfo;

        public SimpleExpression(string propertyName, Operator @operator, string propertyValue)
            : this(propertyName, null, @operator, propertyValue)
        {
        }

        public SimpleExpression(string propertyName, Not not, Operator @operator, string propertyValue)
        {
            _propertyName = propertyName;
            _not = not;
            _operator = @operator;
            _propertyValue = propertyValue;
        }

        public override bool Evaluate(Context context)
        {
            ExtractPropertyInfo(context);
            Boolean value = EvaluateExpression(context);
            if (_not != null)
                value = !value;
            return value;
        }

        private void ExtractPropertyInfo(Context context)
        {
            _propertyInfo = context.Entry.GetType().GetProperty(_propertyName);
            if (_propertyInfo == null)
                throw new InterpreterException("Property " + _propertyName + " does not exist.");
        }

        private bool EvaluateExpression(Context context)
        {
            return _operator.Evaluate(_propertyInfo.GetValue(context.Entry,null), _propertyValue);
        }
    }
}