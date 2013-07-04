using System;
using YalvLib.Common.Exceptions;


namespace YalvLib.Model.Filter
{
    public class BinaryExpression : BooleanExpression
    {
        private readonly BooleanExpression _expressionLeft;
        private readonly BooleanExpression _expressionRight;
        private readonly BinaryOperator _operator;

        public BinaryExpression(BooleanExpression expressionLeft, BinaryOperator @operator,
                                BooleanExpression expressionRight)
        {
            if(expressionLeft == null || expressionRight == null || @operator == null)
                throw  new InterpreterException("expressions or operator is null");
            _expressionLeft = expressionLeft;
            _expressionRight = expressionRight;
            _operator = @operator;
        }

        public override bool Evaluate(Context context)
        {
            Boolean expressionLeftValue = _expressionLeft.Evaluate(context);
            Boolean expressionRightValue = _expressionRight.Evaluate(context);
            return _operator.Evaluate(expressionLeftValue, expressionRightValue);
        }
    }
}