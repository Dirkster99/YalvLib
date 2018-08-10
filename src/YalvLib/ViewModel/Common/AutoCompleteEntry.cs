namespace YalvLib.ViewModel.Common
{
    /// <summary>
    /// Class is used to display a help string for typing a filter
    /// </summary>
    public class AutoCompleteEntry
    {
        #region fields
        private string[] _keywordStrings;
        private string _displayString;
        #endregion fields

        #region constructors
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
        #endregion constructors

        #region methods
        /// <summary>
        /// Gets the table of keywords
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
        /// Gets/Sets the display string
        /// </summary>
        public string DisplayName
        {
            get { return _displayString; }
            set { _displayString = value; }
        }

        /// <summary>
        /// Standard method to support debugging and display raw data stored in this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _displayString;
        }
        #endregion methods
    }
}
