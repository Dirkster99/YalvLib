using Irony.Parsing;
using YalvLib.Common.Exceptions;
using YalvLib.Infrastructure.Filter;
using YalvLib.Model.Filter;

namespace YalvLib.Common.Converter
{
    public class StringConverter
    {
        private readonly YalvGrammar _grammar;
        private string _query;
        private ParseTreeNode _queryTree;
        private BooleanExpression _expression;
        private readonly Parser _parser;

        public StringConverter(string query)
        {
            _query = query;
            _grammar = new YalvGrammar();
            _parser = new Parser(_grammar);
            Parse();
        }

        public StringConverter()
        {
            _grammar = new YalvGrammar();
            _parser = new Parser(_grammar);
        }

        public ParseTreeNode Parse()
        {
            if(_query == null)
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

        public string Query
        {
            get { return _query; }
            set { if (value != null) _query = value; }
        }

        public BooleanExpression Convert()
        {
            BooleanExpression coin = BuildBooleanExpression(Parse());
            return coin;
        }

        private BooleanExpression BuildBooleanExpression(ParseTreeNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Term.Equals(_grammar.BinaryExpression))
                    _expression = BuildBinaryExpression(node.ChildNodes[i - 2], node.ChildNodes[i].ChildNodes[0].Term,
                                                        node.ChildNodes[i + 2]);
                else if (node.ChildNodes[i].Term.Equals(_grammar.CondEval))
                    _expression = BuildSimpleExpression(node.ChildNodes[i]);
            }
            return _expression;
        }

        private BooleanExpression BuildSimpleExpression(ParseTreeNode node)
        {
            if (node.ChildNodes.Count > 3)
                return new SimpleExpression(node.ChildNodes[0].ChildNodes[0].Term.ToString(), new Not(),
                                            GetOperator(node.ChildNodes[2]), node.ChildNodes[3].Token.Value.ToString());
            
            return new SimpleExpression(node.ChildNodes[0].ChildNodes[0].Term.ToString(),
                                        GetOperator(node.ChildNodes[1]), 
                                        node.ChildNodes[2].Term.Equals(_grammar.DateValue)?BuildDateValue(node.ChildNodes[2])
                                        :node.ChildNodes[2].Token.Value.ToString());
        }

        private string BuildDateValue(ParseTreeNode node)
        {
            string dateValue = string.Empty;
            foreach(var childNode in node.ChildNodes)
            {
                dateValue += childNode.Token.Value.ToString();
            }
            return dateValue;
        }

        private Operator GetOperator(ParseTreeNode p)
        {
            if (p.ChildNodes[0].Term.Equals(_grammar.EQUALS))
                return new EqualsOperator();
            if (p.ChildNodes[0].Term.Equals(_grammar.BEFORE))
                    return new BeforeOperator();
            if(p.ChildNodes[0].Term.Equals(_grammar.AFTER))
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