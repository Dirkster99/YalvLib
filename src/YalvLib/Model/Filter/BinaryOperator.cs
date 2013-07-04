namespace YalvLib.Model.Filter
{
    public abstract class BinaryOperator
    {
        public abstract bool Evaluate(bool leftValue, bool rightValue);
    }
}