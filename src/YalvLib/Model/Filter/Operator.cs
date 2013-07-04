namespace YalvLib.Model.Filter
{
    public abstract class Operator
    {
        public abstract bool Evaluate(object property, string value);
    }
}