using Irony.Parsing;
using YalvLib.Common.Exceptions;
using YalvLib.Infrastructure.Filter;
using YalvLib.Model.Filter;

namespace YalvLib.Common.Converter
{
    /// <summary>
    /// string to Filter Query Converter
    /// </summary>
    public class StringConverter
    {
        private readonly YalvGrammar _grammar;
        private readonly Parser _parser;
        private BooleanExpression _expression;
        private string _query;
        private ParseTreeNode _queryTree;

        /// <summary>
        /// Constructor
        /// Get the defined grammar, instantiate a parser and build the tree from the query
        /// </summary>
        /// <param name="query">Query to convert</param>
        public StringConverter(string query)
        {
            _query = query;
            _grammar = new YalvGrammar();
            _parser = new Parser(_grammar);
            Parse();
        }

        /// <summary>
        /// Constructor
        /// Get the defined grammar, instantiate a parser
        /// </summary>
        public StringConverter()
        {
            _grammar = new YalvGrammar();
            _parser = new Parser(_grammar);
        }

        /// <summary>
        /// getter / setter of the query to parse
        /// </summary>
        public string Query
        {
            get { return _query; }
            set { if (value != null) _query = value; }
        }

        /// <summary>
        /// Build the tree from the query with the parser
        /// </summary>
        /// <returns>Top tree node</returns>
        public ParseTreeNode Parse()
        {
            if (_query == null)
            {
                throw new InterpreterException("The query is undefined");
            }
            ParseTree parseTree = _parser.Parse(_query);
            if (parseTree == null)
            {
                throw new InterpreterException("Query invalid");
            }
            if (parseTree.Root == null || parseTree.Root.ChildNodes.Count == 0)
                return null;
            _queryTree = parseTree.Root.ChildNodes[0];
            return _queryTree;
        }

        /// <summary>
        /// Build the expression from the builded treee
        /// </summary>
        /// <returns></returns>
        public BooleanExpression Convert()
        {
            return BuildBooleanExpression(Parse());
        }


        private BooleanExpression BuildBooleanExpression(ParseTreeNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Term.Equals(_grammar.BinaryExpression))
                    _expression = BuildBinaryExpression(node.ChildNodes[i - 2], node.ChildNodes[i].ChildNodes[0].Term,
                                                        node.ChildNodes[i + 2]); // i-2 = leftExpr i+2 = rightExpr i = operator
                else if (node.ChildNodes[i].Term.Equals(_grammar.CondEval))
                    _expression = BuildSimpleExpression(node.ChildNodes[i]);
            }
            return _expression;
        }

        private BooleanExpression BuildSimpleExpression(ParseTreeNode node)
        {
            // HAS TextMarker is not implemented yet
            if (node.ChildNodes.Count > 3) // means we have a NOT expr
                return new SimpleExpression(node.ChildNodes[0].ChildNodes[0].Term.ToString(), new Not(),
                                            GetOperator(node.ChildNodes[2]), node.ChildNodes[3].Token.Value.ToString());

            return new SimpleExpression(node.ChildNodes[0].ChildNodes[0].Term.ToString(),
                                        GetOperator(node.ChildNodes[1]),
                                        node.ChildNodes[2].Term.Equals(_grammar.DateValue)
                                            ? BuildDateValue(node.ChildNodes[2])
                                            : node.ChildNodes[2].Token.Value.ToString());
                // Return the simple expression depending if it's a date query or not
        }

        private string BuildDateValue(ParseTreeNode node)
        {
            string dateValue = string.Empty;
            foreach (ParseTreeNode childNode in node.ChildNodes)
            {
                dateValue += childNode.Token.Value.ToString();
            }
            return dateValue;
        }

        /// <summary>
        /// Return the operator of the node
        /// </summary>
        private Operator GetOperator(ParseTreeNode p)
        {
            if (p.ChildNodes[0].Term.Equals(_grammar.EQUALS))
                return new EqualsOperator();
            if (p.ChildNodes[0].Term.Equals(_grammar.CONTAINS))
                return new ContainsOperator();
            if (p.ChildNodes[0].Term.Equals(_grammar.BEFORE))
                return new BeforeOperator();
            if (p.ChildNodes[0].Term.Equals(_grammar.AFTER))
                return new AfterOperator();
            throw new InterpreterException("Unknown Operator");
        }

        private BinaryOperator GetBinaryOperator(BnfTerm p)
        {
            if (p.Equals(_grammar.OR))
                return new Or();
            if (p.Equals(_grammar.AND))
                return new And();
            throw new InterpreterException("Unknown binary Operator");
        }


        private BooleanExpression BuildBinaryExpression(ParseTreeNode leftExpression, BnfTerm @operator,
                                                        ParseTreeNode rightExpression)
        {
            return new BinaryExpression(BuildBooleanExpression(leftExpression), GetBinaryOperator(@operator),
                                        BuildBooleanExpression(rightExpression));
        }
    }
}