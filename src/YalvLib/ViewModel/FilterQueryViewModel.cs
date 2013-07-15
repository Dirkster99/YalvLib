using System;
using YalvLib.Common;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// Class that represent a filter query
    /// </summary>
    public class FilterQueryViewModel : BindableObject
    {
        private readonly string _queryString;
        private bool _active;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="query">Query to create the instance from</param>
        public FilterQueryViewModel(string query)
        {
            _queryString = query;
            _active = true;
            CommandCancelQuery = new CommandRelay(ExecuteCancelQuery, CanExecuteCancelQuery);
        }

        /// <summary>
        /// Return the query filter
        /// </summary>
        public string QueryString
        {
            get { return _queryString; }
        }

        /// <summary>
        /// Command if the query is to be deleted
        /// </summary>
        public CommandRelay CommandCancelQuery { get; private set; }

        /// <summary>
        /// Getter / Setter active property
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set
            {
                if (value != _active)
                {
                    _active = value;
                    NotifyPropertyChanged(() => Active);
                }
            }
        }

        /// <summary>
        /// Delete a query
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ExecuteCancelQuery(object obj)
        {
            OnExecutedCancelQuery();
            return null;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        public bool CanExecuteCancelQuery(object obj)
        {
            return true;
        }

        /// <summary>
        /// Event raised if a query is deleted
        /// </summary>
        public event EventHandler QueryDeleted;

        private void OnExecutedCancelQuery()
        {
            if (QueryDeleted != null)
            {
                QueryDeleted(this, null);
            }
        }
    }
}