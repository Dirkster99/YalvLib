namespace Filters
{
    using Irony.Parsing;

    /// <summary>
    /// Class that represent the Irony parser grammar used by the filter feature in Yalv
    ///
    /// Terminal symbols are elements that are predefined for grammar rules - they
    /// don't have a definition in the grammar. Terminals represent primary indivisible
    /// words (tokens) in the input stream and are produced by an input pre-processor
    /// called the Scanner or Lexer.
    ///
    /// Non-terminals, on the other hand, are compound elements, and their internal
    /// structure is defined by BNF expressions.
    ///
    /// https://www.codeproject.com/Articles/22650/Irony-NET-Compiler-Construction-Kit
    /// </summary>
    public class YalvGrammar : Grammar
    {
        /// <summary>
        /// Implement a AND terminal property for a grammar parser.
        /// </summary>
        public Terminal AND = null;

        /// <summary>
        /// Implement a OR terminal property for a grammar parser.
        /// </summary>
        public Terminal OR = null;

        /// <summary>
        /// Implement a NOT terminal property for a grammar parser.
        /// </summary>
        public Terminal NOT = null;

        /// <summary>
        /// Implement a ID terminal property for a grammar parser.
        /// </summary>
        public Terminal ID = null;

        /// <summary>
        /// Implement a APP terminal property for a grammar parser.
        /// </summary>
        public Terminal APP = null;

        /// <summary>
        /// Implement a LOGGER terminal property for a grammar parser.
        /// </summary>
        public Terminal LOGGER = null;

        /// <summary>
        /// Implement a MESSAGE terminal property for a grammar parser.
        /// </summary>
        public Terminal MESSAGE = null;

        /// <summary>
        /// Implement a THREAD terminal property for a grammar parser.
        /// </summary>
        public Terminal THREAD = null;

        /// <summary>
        /// Implement a CLASS terminal property for a grammar parser.
        /// </summary>
        public Terminal CLASS = null;

        /// <summary>
        /// Implement a METHOD terminal property for a grammar parser.
        /// </summary>
        public Terminal METHOD = null;

        /// <summary>
        /// Implement a MACHINENAME terminal property for a grammar parser.
        /// </summary>
        public Terminal MACHINENAME = null;

        /// <summary>
        /// Implement a USERNAME terminal property for a grammar parser.
        /// </summary>
        public Terminal USERNAME = null;

        /// <summary>
        /// Implement a THROWABLE terminal property for a grammar parser.
        /// </summary>
        public Terminal THROWABLE = null;

        /// <summary>
        /// Implement a FILE terminal property for a grammar parser.
        /// </summary>
        public Terminal FILE = null;

        /// <summary>
        /// Implement a LIEN terminal property for a grammar parser.
        /// </summary>
        public Terminal LINE = null;

        /// <summary>
        /// Implement a TEXT MARKER MESSAGE terminal property for a grammar parser.
        /// </summary>
        public Terminal TEXTMARKERMESSAGE = null;

        /// <summary>
        /// Implement a TEXT MARKE RAUTHOR terminal property for a grammar parser.
        /// </summary>
        public Terminal TEXTMARKERAUTHOR = null;

        /// <summary>
        /// Implement a TEXTMARKER terminal property for a grammar parser.
        /// </summary>
        public Terminal TEXTMARKER = null;

        /// <summary>
        /// Implement a TIMESTAMP terminal property for a grammar parser.
        /// </summary>
        public Terminal TIMESTAMP = null;

        /// <summary>
        /// Implement a TEXT MARKER CREATION terminal property for a grammar parser.
        /// </summary>
        public Terminal TEXTMARKERCREATION = null;

        /// <summary>
        /// Implement a TEXT MARKER MODIFICATION terminal property for a grammar parser.
        /// </summary>
        public Terminal TEXTMARKERMODIFICATION = null;

        /// <summary>
        /// Implement a HAS terminal property for a grammar parser.
        /// </summary>
        public Terminal HAS = null;

        /// <summary>
        /// Implement a EQUALS terminal property for a grammar parser.
        /// </summary>
        public Terminal EQUALS = null;

        /// <summary>
        /// Implement a CONTAINS terminal property for a grammar parser.
        /// </summary>
        public Terminal CONTAINS = null;

        /// <summary>
        /// Implement a BEFORE terminal property for a grammar parser.
        /// </summary>
        public Terminal BEFORE = null;

        /// <summary>
        /// Implement a AFTER terminal property for a grammar parser.
        /// </summary>
        public Terminal AFTER = null;

        /// <summary>
        /// Implement a BracketOpen terminal property for a grammar parser.
        /// </summary>
        public Terminal BracketOpen = null;

        /// <summary>
        /// Implement a BracketClose terminal property for a grammar parser.
        /// </summary>
        public Terminal BracketClose = null;

        #region Non Terminals
        /// <summary>
        /// Implement a S Non-terminal property for a grammar parser.
        /// </summary>
        public NonTerminal S = new NonTerminal("S");

        /// <summary>
        /// Implement an Expression terminal property for a grammar parser.
        /// </summary>
        public NonTerminal Expression = new NonTerminal("Expression");

        /// <summary>
        /// Implement a CondEval terminal property for a grammar parser.
        /// </summary>
        public NonTerminal CondEval = new NonTerminal("CondEval");

        /// <summary>
        /// Implement a Property terminal property for a grammar parser.
        /// </summary>
        public NonTerminal Property = new NonTerminal("Property");

        /// <summary>
        /// Implement a Binary Expression terminal property for a grammar parser.
        /// </summary>
        public NonTerminal BinaryExpression = new NonTerminal("BinaryExpression");

        /// <summary>
        /// Implement a Date Cond terminal property for a grammar parser.
        /// </summary>
        public NonTerminal DateCond = new NonTerminal("DateCond");

        /// <summary>
        /// Implement a Cond terminal property for a grammar parser.
        /// </summary>
        public NonTerminal Cond = new NonTerminal("Cond");

        /// <summary>
        /// Implement a Date Property terminal property for a grammar parser.
        /// </summary>
        public NonTerminal DateProperty = new NonTerminal("DateProperty");

        /// <summary>
        /// Implement a DateValue terminal property for a grammar parser.
        /// </summary>
        public NonTerminal DateValue = new NonTerminal("DateValue");

        /// <summary>
        /// Implement a DateSeparator terminal property for a grammar parser.
        /// </summary>
        public NonTerminal DateSeparator = new NonTerminal("DateSeparator");
        #endregion Non Terminals

        #region RegexBasedTerminals
        /// <summary>
        /// Implement a Value terminal property for a grammar parser.
        /// </summary>
        public Terminal Value = new RegexBasedTerminal("stringValue", @"[\w+.?|\s?]+");

        /// <summary>
        /// Implement a year value terminal property for a grammar parser.
        /// </summary>
        public Terminal yearValue = new RegexBasedTerminal("YearValue", @"\d{4}");

        /// <summary>
        /// Implement a month value terminal property for a grammar parser.
        /// </summary>
        public Terminal monthValue = new RegexBasedTerminal("MonthValue", @"0[1-9]|1[012]");

        /// <summary>
        /// Implement a day value terminal property for a grammar parser.
        /// </summary>
        public Terminal dayValue = new RegexBasedTerminal("DayValue", @"[012][0-9]|3[01]");

        /// <summary>
        /// Implement a hour value terminal property for a grammar parser.
        /// </summary>
        public Terminal hourValue = new RegexBasedTerminal("HourValue", @"\d{2}");

        /// <summary>
        /// Implement a Min Sec value terminal property for a grammar parser.
        /// </summary>
        public Terminal MinSecValue = new RegexBasedTerminal("MinSecValue", @"\d{2}");

        /// <summary>
        /// Implement a t Separator terminal property for a grammar parser.
        /// </summary>
        public Terminal tSeparator = new RegexBasedTerminal("dateSeparator", "T");
        #endregion RegexBasedTerminals

        /// <summary>
        /// Class constructor.
        /// </summary>
        public YalvGrammar() : base(false)
        {
            // Terminals
            AND = ToTerm("AND");
            OR = ToTerm("OR");
            NOT = ToTerm("NOT");

            ID = ToTerm("Id");
            APP = ToTerm("App");
            LOGGER = ToTerm("Logger");
            MESSAGE = ToTerm("Message");
            MACHINENAME = ToTerm("MachineName");
            USERNAME = ToTerm("Username");
            THREAD = ToTerm("Thread");
            CLASS = ToTerm("Class");
            METHOD = ToTerm("Method");
            THROWABLE = ToTerm("Throwable");
            FILE = ToTerm("File");
            LINE = ToTerm("Line");
            TEXTMARKER = ToTerm("TextMarker");
            TEXTMARKERMESSAGE = ToTerm("TextMarkerMessage");
            TEXTMARKERAUTHOR = ToTerm("TextMarkerAuthor");

            TIMESTAMP = ToTerm("TimeStamp");
            TEXTMARKERCREATION = ToTerm("TextMarkerCreation");
            TEXTMARKERMODIFICATION = ToTerm("TextMarkerModification");

            EQUALS = ToTerm("EQUALS");
            CONTAINS = ToTerm("CONTAINS");
            HAS = ToTerm("HAS");

            BEFORE = ToTerm("BEFORE");
            AFTER = ToTerm("AFTER");

            BracketOpen = ToTerm("(");
            BracketClose = ToTerm(")");

            //var whitespaceSeparator = new RegexBasedTerminal("whiteSpaceSeparator", @"");           

            Root = S;

            S.Rule = Expression | Empty;

            Expression.Rule = CondEval
                              |
                              BracketOpen + Expression + BracketClose + BinaryExpression + BracketOpen + Expression +
                              BracketClose;
                              
            CondEval.Rule = Property + Cond + Value
                | Property + NOT + Cond + Value 
                | DateProperty + DateCond + DateValue
                | HAS + TEXTMARKER
                | HAS + NOT + TEXTMARKER;

            BinaryExpression.Rule = AND | OR;
            
            Property.Rule = ID | APP | LOGGER | MESSAGE | MACHINENAME | 
                THREAD | CLASS | METHOD | USERNAME | THROWABLE | FILE | 
                LINE | TEXTMARKERMESSAGE | TEXTMARKERAUTHOR | TEXTMARKER;

            DateProperty.Rule = TIMESTAMP | TEXTMARKERCREATION | TEXTMARKERMODIFICATION;

            Cond.Rule = EQUALS | CONTAINS;

            DateCond.Rule = BEFORE | AFTER | EQUALS;

            DateValue.Rule = yearValue + "-" + monthValue + "-" + dayValue + tSeparator + hourValue + ":" +
                             MinSecValue + ":" + MinSecValue
                             | hourValue + ":" + MinSecValue + ":" + MinSecValue;
        }
    }
}
