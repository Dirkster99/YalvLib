﻿namespace Filters.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Filters.Exceptions;

    /// <summary>
    /// Represent a simple expression
    /// eg property operator value
    /// </summary>
    public class SimpleExpression : BooleanExpression
    {
        #region fields
        private readonly Not _not;
        private readonly Operator _operator;
        private readonly string _propertyName;
        private readonly string _propertyValue;
        private PropertyInfo _propertyInfo;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyName">Property name to get value from</param>
        /// <param name="operator">operator to evalute with</param>
        /// <param name="propertyValue">value to check</param>
        public SimpleExpression(string propertyName, Operator @operator, string propertyValue)
            : this(propertyName, null, @operator, propertyValue)
        {
        }

        /// <summary>
        /// Constructor with NOT 
        /// </summary>
        /// <param name="propertyName">Property name to get value from</param>
        ///<param name="not"></param>
        /// <param name="operator">operator to evalute with</param>
        /// <param name="propertyValue">value to check</param>
        public SimpleExpression(string propertyName, Not not, Operator @operator, string propertyValue)
        {
            _propertyName = propertyName;
            _not = not;
            _operator = @operator;
            _propertyValue = propertyValue;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Evalute the boolean expression based on the given context.
        /// 
        /// Then apply the not operator if present and return the
        /// boolean (true/false) result of this expression's evaluation.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool Evaluate(Context context)
        {
            ExtractPropertyInfo(context);
            Boolean value = EvaluateExpression(context);

            if (_not != null)
                value = !value;

            return value;
        }

        private void ExtractPropertyInfo(Context context)
        {
            _propertyInfo = context.Entry.GetType().GetProperty(_propertyName);

            if (_propertyInfo == null &&
                (_propertyName.IndexOf("textmarker", StringComparison.OrdinalIgnoreCase) < 0))
                throw new InterpreterException("Property " + _propertyName + " does not exist.");
        }

        /// <summary>
        /// If the given property is not directly defined in the model we use this function
        /// Case is ignored for the property name
        /// </summary>
        private List<object> ExtractCustomProperty(Context context)
        {
            var result = new List<object>();
            if (_propertyName.Equals("TextMarkerMessage"))
                result.AddRange(context.Analysis.GetTextMarkersForEntry(context.Entry).Select(marker => marker.Message));

            if (_propertyName.Equals("TextMarkerAuthor"))
                result.AddRange(context.Analysis.GetTextMarkersForEntry(context.Entry).Select(marker => marker.Author));

            if (_propertyName.Equals("TextMarkerCreation"))
                result.AddRange(context.Analysis.GetTextMarkersForEntry(context.Entry).Select(marker => marker.DateCreation.ToString()));

            if (_propertyName.Equals("TextMarkerModification"))
                result.AddRange(context.Analysis.GetTextMarkersForEntry(context.Entry).Select(marker => marker.DateLastModification.ToString()));

            return result;
        }

        private bool EvaluateExpression(Context context)
        {
            if (_propertyInfo != null)
                return _operator.Evaluate(_propertyInfo.GetValue(context.Entry, null), _propertyValue);

            return _operator.Evaluate(ExtractCustomProperty(context), _propertyValue);
        }
        #endregion Methods
    }
}