namespace Filters.Models
{
    using System;
    using Filters.Exceptions;

    /// <summary>
    /// Represent a binary expression
    /// eg expression operator (OR / AND) expression
    /// </summary>
    public class BinaryExpression : BooleanExpression
    {
        private readonly BooleanExpression _expressionLeft;
        private readonly BooleanExpression _expressionRight;
        private readonly BinaryOperator _operator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expressionLeft">Left expression</param>
        /// <param name="operator">operator (AND OR)</param>
        /// <param name="expressionRight">Right Expression</param>
        public BinaryExpression(BooleanExpression expressionLeft, BinaryOperator @operator,
                                BooleanExpression expressionRight)
        {
            if(expressionLeft == null || expressionRight == null || @operator == null)
                throw  new InterpreterException("expressions or operator is null");
            _expressionLeft = expressionLeft;
            _expressionRight = expressionRight;
            _operator = @operator;
        }

        /// <summary>
        /// Evaluate the left and right expression with the given context
        /// Then apply the operator to the result of both expressions
        /// </summary>
        /// <param name="context">Given context (Entry + Analysis)</param>
        /// <returns></returns>
        public override bool Evaluate(Context context)
        {
            Boolean expressionLeftValue = _expressionLeft.Evaluate(context);
            Boolean expressionRightValue = _expressionRight.Evaluate(context);
            return _operator.Evaluate(expressionLeftValue, expressionRightValue);
        }
    }
}