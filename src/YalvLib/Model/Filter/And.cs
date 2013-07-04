namespace YalvLib.Model.Filter
{
    public class And : BinaryOperator
    {
        public override bool Evaluate(bool leftValue, bool rightValue)
        {
            return leftValue && rightValue;
        }
    }
}