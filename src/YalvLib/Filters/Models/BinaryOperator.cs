namespace Filters.Models
{
    /// <summary>
    /// Abstract class of a binary operator
    /// Imeplenting the common methods
    /// </summary>
    public abstract class BinaryOperator
    {
        /// <summary>
        /// Evaluate the left and right values
        /// </summary>
        /// <param name="leftValue">Value of the left child</param>
        /// <param name="rightValue">Value of the right child</param>
        /// <returns></returns>
        public abstract bool Evaluate(bool leftValue, bool rightValue);
    }
}