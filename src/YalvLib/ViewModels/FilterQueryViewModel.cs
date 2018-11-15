namespace YalvLib.ViewModels
{
    using System;
    using YalvLib.Common;
    using YalvLib.Model;

    /// <summary>
    /// Class that represent a filter query
    /// </summary>
    public class FilterQueryViewModel : BindableObject
    {
        private CustomFilter _filter;
        private bool _active;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="query">Query to create the instance from</param>
        public FilterQueryViewModel(string query)
        {
            _filter = new CustomFilter(query);
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddFilter(_filter);
            _active = true;
            CommandCancelQuery = new CommandRelay(ExecuteCancelQuery, CanExecuteCancelQuery);
        }

        /// <summary>
        /// Ctor from an existing log analysis
        /// </summary>
        /// <param name="filter"></param>
        public FilterQueryViewModel(CustomFilter filter)
        {
            _filter = filter;
            _active = false;
            CommandCancelQuery = new CommandRelay(ExecuteCancelQuery, CanExecuteCancelQuery);
        }

        /// <summary>
        /// Return the query filter
        /// </summary>
        public string QueryString
        {
            get { return _filter.Value; }
        }

        /// <summary>
        /// Return the filter
        /// </summary>
        public CustomFilter Filter
        {
            get { return _filter; }
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