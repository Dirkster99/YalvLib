using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
using YalvLib.Common;
using YalvLib.Model;
using YalvLib.Model.Filter;
using YalvLib.ViewModel.Common;
using StringConverter = YalvLib.Common.Converter.StringConverter;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// Parse the filter query and evaluate with a specific context
    /// </summary>
    public class FilterConverterViewModel : BindableObject
    {
        private readonly StringConverter _converter;
        private readonly List<FilterQueryViewModel> _queries;
        private Context _context;
        private List<AutoCompleteEntry> _autoCompleteList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logAnalysis">Log analysis for the textmarkers informations</param>
        public FilterConverterViewModel(LogAnalysis logAnalysis)
        {
            _converter = new StringConverter();
            _context = new Context {Analysis = logAnalysis};
            _queries = new List<FilterQueryViewModel>();
            GenerateFiltersFromAnalysis(logAnalysis);
            ActualQuery = string.Empty;
            InitAutoCompleteList();
        }

        private void GenerateFiltersFromAnalysis(LogAnalysis logAnalysis)
        {
            foreach (var filter in logAnalysis.Filters)
            {
                AddQuery(filter);
            }
        }

        private void InitAutoCompleteList()
        {
            var entry = new LogEntry();
            _autoCompleteList = new List<AutoCompleteEntry>();
            AutoCompleteList.CollectionChanged += (sender, args) => NotifyPropertyChanged(() => AutoCompleteList);
            foreach (var property in entry.GetType().GetProperties())
            {
                _autoCompleteList.Add(new AutoCompleteEntry(property.Name, null));
            }
        }

        /// <summary>
        /// Getter / Setter of the query present in the converter
        /// </summary>
        public string ActualQuery
        {
            get { return _converter.Query; }
            set { if (value != null) _converter.Query = value; }
        }

        /// <summary>
        /// Return the list of queries entered by the user
        /// </summary>
        public ObservableCollection<FilterQueryViewModel> Queries
        {
            get
            {
                if (_queries != null)
                    return new ObservableCollection<FilterQueryViewModel>(_queries);
                return new ObservableCollection<FilterQueryViewModel>();
            }
        }

        public ObservableCollection<AutoCompleteEntry> AutoCompleteList
        {
            get { return new ObservableCollection<AutoCompleteEntry>(_autoCompleteList); }
        }

        /// <summary>
        /// Getter / Setter of the context
        /// </summary>
        public Context Context
        {
            get { return _context; }
            set { _context = value;}
        }

        public LogAnalysis Analysis
        {
            get { return _context.Analysis; }
            set { if (value != null && value != Analysis){ _context.Analysis = value;GenerateFiltersFromAnalysis(Analysis);} }
        }

        /// <summary>
        /// Add the query to the list of queries
        /// </summary>
        /// <param name="query">Query to add</param>
        public void AddQuery(string query)
        {
            var querytoAdd = new FilterQueryViewModel(query);
            _queries.Add(querytoAdd);
            querytoAdd.QueryDeleted += ExecuteCancel;
            querytoAdd.PropertyChanged += (sender, args) => NotifyPropertyChanged(() => Queries);
            NotifyPropertyChanged(() => Queries);
        }

        public void AddQuery(CustomFilter filter)
        {
            var filtertoAdd = new FilterQueryViewModel(filter);
            _queries.Add(filtertoAdd);
            filtertoAdd.QueryDeleted += ExecuteCancel;
            filtertoAdd.PropertyChanged += (sender, args) => NotifyPropertyChanged(() => Queries);
            NotifyPropertyChanged(() => Queries);
        }

        /// <summary>
        /// Set the context analysis
        /// </summary>
        /// <param name="logAnalysis"></param>
        public void SetAnalysis(LogAnalysis logAnalysis)
        {
            _context.Analysis = logAnalysis;
        }

        /// <summary>
        /// Tells if the given entry match the set of filters
        /// </summary>
        /// <param name="entry">entry to check</param>
        /// <returns>true if the entry match</returns>
        public Boolean Evaluate(LogEntry entry)
        {
            _context.Entry = entry;
            return ApplyFilter();
        }

        /// <summary>
        /// Tells if the actual query passes the grammar
        /// </summary>
        /// <returns>true if valid, false otherwise</returns>
        public Boolean IsQueryValid()
        {
            _converter.Query = ActualQuery;
            return _converter.Parse() != null;
        }

        /// <summary>
        /// Apply the set of filter to the current context
        /// </summary>
        /// <returns>true if the context match</returns>
        public Boolean ApplyFilter()
        {
            foreach (FilterQueryViewModel query in _queries)
            {
                if (query.Active)
                {
                    _converter.Query = query.QueryString;
                    if (!_converter.Convert().Evaluate(_context) && query.QueryString != "")
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Remove the query from the list of queries
        /// </summary>
        /// <param name="obj">Query to remove</param>
        /// <param name="eventArgs"></param>
        public void ExecuteCancel(object obj, EventArgs eventArgs)
        {
            var query = obj as FilterQueryViewModel;
            if (query == null) return;
            query.QueryDeleted -= ExecuteCancel;
            _queries.Remove(query);
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.RemoveFilter(query.Filter);
            NotifyPropertyChanged(() => Queries);
        }
    }
}