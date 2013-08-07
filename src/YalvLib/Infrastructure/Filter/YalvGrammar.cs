using Irony.Parsing;

namespace YalvLib.Infrastructure.Filter
{
    /// <summary>
    /// Class that represent the grammar used by the filter feature in Yalv
    /// </summary>
    public class YalvGrammar : Irony.Parsing.Grammar
    {
        public Terminal AND = null;
        public Terminal OR = null;
        public Terminal NOT = null;

        public Terminal ID = null;
        public Terminal APP = null;
        public Terminal LOGGER = null;
        public Terminal MESSAGE = null;
        public Terminal THREAD = null;
        public Terminal CLASS = null;
        public Terminal METHOD = null;
        public Terminal MACHINENAME = null;
        public Terminal USERNAME = null;
        public Terminal THROWABLE = null;
        public Terminal FILE = null;
        public Terminal LINE = null;
        public Terminal TEXTMARKERMESSAGE = null;
        public Terminal TEXTMARKERAUTHOR = null;
        public Terminal TEXTMARKER = null;
        public Terminal TIMESTAMP = null;
        public Terminal TEXTMARKERCREATION = null;
        public Terminal TEXTMARKERMODIFICATION = null;

        public Terminal HAS = null;
        public Terminal EQUALS = null;
        public Terminal CONTAINS = null;
        public Terminal BEFORE = null;
        public Terminal AFTER = null;
        public Terminal BracketOpen = null;
        public Terminal BracketClose = null;

        // Non terminals
        public NonTerminal S = new NonTerminal("S");
        public NonTerminal Expression = new NonTerminal("Expression");
        public NonTerminal CondEval = new NonTerminal("CondEval");
        public NonTerminal Property = new NonTerminal("Property");
        public NonTerminal BinaryExpression = new NonTerminal("BinaryExpression");
        public NonTerminal DateCond = new NonTerminal("DateCond");
        public NonTerminal Cond = new NonTerminal("Cond");
        public NonTerminal DateProperty = new NonTerminal("DateProperty");
        public NonTerminal DateValue = new NonTerminal("DateValue");
        public NonTerminal DateSeparator = new NonTerminal("DateSeparator");

        // RegexBasedTerminals
        public Terminal Value = new RegexBasedTerminal("stringValue", @"[\w+.?|\s?]+");
        public Terminal yearValue = new RegexBasedTerminal("YearValue", @"\d{4}");
        public Terminal monthValue = new RegexBasedTerminal("MonthValue", @"0[1-9]|1[012]");
        public Terminal dayValue = new RegexBasedTerminal("DayValue", @"[012][0-9]|3[01]");
        public Terminal hourValue = new RegexBasedTerminal("HourValue", @"\d{2}");
        public Terminal MinSecValue = new RegexBasedTerminal("MinSecValue", @"\d{2}");
        public Terminal tSeparator = new RegexBasedTerminal("dateSeparator", "T");



        public YalvGrammar() : base(false)
        {
            //Terminals
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
