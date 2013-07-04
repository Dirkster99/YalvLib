namespace YalvLib.Model.Filter
{
    public abstract class BooleanExpression
    {
        public abstract bool Evaluate(Context context);
    }
}