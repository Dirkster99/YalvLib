namespace YalvLib.Model.Filter
{
    public class Or : BinaryOperator
    {
        public override bool Evaluate(bool leftValue, bool rightValue)
        {
            return leftValue || rightValue;
        }
    }
}