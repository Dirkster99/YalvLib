namespace YalvLib.ViewModel.Common
{
    /// <summary>
    /// Class used to display an help for typing a filter
    /// </summary>
    public class AutoCompleteEntry
    {
        private string[] _keywordStrings;
        private string _displayString;

        /// <summary>
        /// Getter / Setter for the table of keywords
        /// </summary>
        public string[] KeywordStrings
        {
            get
            {
                if (_keywordStrings == null)
                {
                    _keywordStrings = new string[] { _displayString };
                }
                return _keywordStrings;
            }
        }

        /// <summary>
        /// Getter / Setter displayString
        /// </summary>
        public string DisplayName
        {
            get { return _displayString; }
            set { _displayString = value; }
        }

        /// <summary>
        /// Constructor of the autocompleteentry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keywords"></param>
        public AutoCompleteEntry(string name, params string[] keywords)
        {
            _displayString = name;
            _keywordStrings = keywords;
        }

        public override string ToString()
        {
            return _displayString;
        }
    }
}
