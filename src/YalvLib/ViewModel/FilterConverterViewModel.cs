using System;
using System.Collections.Generic;
using YalvLib.Common;
using YalvLib.Common.Converter;
using YalvLib.Common.Exceptions;
using YalvLib.Model;
using YalvLib.Model.Filter;

namespace YalvLib.ViewModel
{
    public class FilterConverterViewModel : BindableObject
    {
        private Context _context;
        private readonly StringConverter _converter;

        public FilterConverterViewModel(LogAnalysis logAnalysis)
        {
            _converter = new StringConverter();
            _context = new Context();
            _context.Analysis = logAnalysis;
            Query = string.Empty;
        }

        public Context Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public string Query
        {
            get { return _converter.Query; }
            set { _converter.Query = value;
                NotifyPropertyChanged(() => Query);
            }
        }

        public void setAnalysis(LogAnalysis logAnalysis)
        {
            _context.Analysis = logAnalysis;
        }

        public Boolean Evaluate(LogEntry entry)
        {
            _context.Entry = entry;
            return ApplyFilter();
        }

        public Boolean IsQueryValid()
        {
            _converter.Query = Query;
            if (_converter.Parse() != null)
                return true;
            return false;
        }

        public Boolean ApplyFilter()
        {
            _converter.Query = Query;
            return _converter.Convert().Evaluate(_context);
        }
    }
}