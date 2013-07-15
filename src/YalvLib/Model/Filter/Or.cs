namespace YalvLib.Model.Filter
{
    /// <summary>
    /// Represent the OR operator
    /// </summary>
    public class Or : BinaryOperator
    {
        /// <summary>
        /// Check if one of the childs is true
        /// </summary>
        /// <param name="leftValue">left child value</param>
        /// <param name="rightValue">right child value</param>
        /// <returns>true if one or both childs are true, false otherwise</returns>
        public override bool Evaluate(bool leftValue, bool rightValue)
        {
            return leftValue || rightValue;
        }
    }
}