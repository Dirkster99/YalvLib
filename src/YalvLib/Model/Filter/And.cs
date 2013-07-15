namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent the key word AND between 2 expression
    /// </summary>
    public class And : BinaryOperator
    {
        /// <summary>
        /// Return the evaluation of his left and right childs
        /// </summary>
        /// <param name="leftValue">left child</param>
        /// <param name="rightValue">right child</param>
        /// <returns>true if both childs are true, false otherwise</returns>
        public override bool Evaluate(bool leftValue, bool rightValue)
        {
            return leftValue && rightValue;
        }
    }
}