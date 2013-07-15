namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent a boolean expression
    /// </summary>
    public abstract class BooleanExpression
    {
        /// <summary>
        /// Evalute the expression based on the given context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>true if the context is valid for the given expression</returns>
        public abstract bool Evaluate(Context context);
    }
}