﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Common.Exceptions;

namespace YalvLib.Model.Filter
{
    /// <summary>
    /// This clqss represent the expression has / has not textmarker
    /// </summary>
    public class HasExpression : BooleanExpression
    {
        private readonly Not _not;

        /// <summary>
        /// constructor without NOT
        /// </summary>
        /// <param name="operatorName">operator has</param>
        /// <param name="propertyName">property textmarker</param>
        public HasExpression(string operatorName, string propertyName):this(operatorName,propertyName, null)
        {
        }

        /// <summary>
        /// Constructor with not
        /// </summary>
        /// <param name="operatorName">operator has</param>
        /// <param name="propertyName">property textmarker</param>
        /// <param name="not">not</param>
        public HasExpression(string operatorName, string propertyName, Not not)
        {
            if (operatorName.IndexOf("has", StringComparison.OrdinalIgnoreCase) < 0)
                throw new InterpreterException("Wrong operator, use the HAS operator please");
            if (propertyName.IndexOf("textmarker", StringComparison.OrdinalIgnoreCase) < 0)
                throw new InterpreterException("Wrong Property, use the TextMarker operator please");
            _not = not;
        }

        public override bool Evaluate(Context context)
        {        
            var result = context.Analysis.GetTextMarkersForEntry(context.Entry).Count > 0;
            if (_not != null)
                return !result;
            return result;
        }
    }
}
