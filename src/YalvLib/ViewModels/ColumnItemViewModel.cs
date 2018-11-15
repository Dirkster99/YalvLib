namespace YalvLib.ViewModels
{
    using log4netLib.Enums;
    using log4netLib.Interfaces;
    using log4netLib.Utils;
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using YalvLib.Common;

    /// <summary>
    /// Implements a Header viewModel for the Header row of a dataGrid
    /// and stores/retrieves layout data about each column.
    /// </summary>
    public class ColumnItemViewModel : BindableObject, IColumnItem, IXmlSerializable
    {
        #region fields

        private string _columnFilterValue = string.Empty;
        private bool _isColumnVisible = true;

        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="minWidth"></param>
        /// <param name="width"></param>
        /// <param name="align"></param>
        /// <param name="stringFormat"></param>
        /// <param name="header"></param>
        /// <param name="isColumnVisible"></param>
        public ColumnItemViewModel(string field,
                          double minWidth, double width,
                          CellAlignment align, string stringFormat,
                          string header,
                          bool isColumnVisible = true)
            : this()
        {
            Field = field;
            MinWidth = minWidth;
            Width = width;
            Alignment = align;
            StringFormat = stringFormat;
            Header = header;

            this.IsColumnVisible = isColumnVisible;
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        protected ColumnItemViewModel()
        {
            FilterControlName = string.Empty;

            ActualWidth = new BindSupport();
            Width = MinWidth = 25;
        }
        #endregion constructor

        #region events

        /// <summary>
        /// This event is raised when a text character is keyed into a filter textbox
        /// </summary>
        public event EventHandler UpdateColumnFilter;

        #endregion events

        #region properties
        /// <summary>
        /// Column header label string
        /// </summary>
        public string Header { get; protected set; }

        /// <summary>
        /// Field name of data source
        /// </summary>
        public string Field { get; protected set; }

        /// <summary>
        /// Name of textbox that is used to filter this column.
        /// This field is used to update the column filter when the user
        /// types/edits the textbox content.
        /// </summary>
        public string FilterControlName { get; protected set; }

        /// <summary>
        /// Get/set string that can be used to filter the view by the contents of this column
        /// </summary>
        public string ColumnFilterValue
        {
            get { return _columnFilterValue; }

            protected set
            {
                if (_columnFilterValue != value)
                {
                    _columnFilterValue = value;
                    RaisePropertyChanged("ColumnFilterValue");

                    if (UpdateColumnFilter != null)
                        UpdateColumnFilter(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get/set string format for dates (and such data types) in localized context. 
        /// </summary>
        public string StringFormat { get; protected set; }

        /// <summary>
        /// Get/set minimum width of this column.
        /// </summary>
        public double MinWidth { get; protected set; }

        /// <summary>
        /// Get/set actual width of this column.
        /// </summary>
        public double Width { get; protected set; }

        /// <summary>
        /// Get property to bind the actual width of a column to
        /// (this readonly dp requires a dependency property which in turn
        /// requires an inheritance from DependencyObject)
        /// </summary>
        public BindSupport ActualWidth { get; private set; }

        /// <summary>
        /// Get/set cell alignment for cells displayed in this column.
        /// </summary>
        public CellAlignment Alignment { get; protected set; }

        /// <summary>
        /// Get/set whether column is visible or not.
        /// </summary>
        public bool IsColumnVisible
        {
            get { return _isColumnVisible; }

            set
            {
                if (_isColumnVisible != value)
                {
                    _isColumnVisible = value;
                    RaisePropertyChanged("IsColumnVisible");
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Sets the the <see cref="ColumnFilterValue"/> property with the given string.
        /// </summary>
        /// <param name="filterString"></param>
        public void SetColumnFilterValue(string filterString)
        {
            this.ColumnFilterValue = filterString;
        }

        /// <summary>
        /// Sets the the <see cref="Width"/> property to the given value.
        /// </summary>
        /// <param name="width"></param>
        public void SetWidth(double width)
        {
            this.Width = width;
        }

        /// <summary>
        /// Sets the the <see cref="FilterControlName"/> property with the given string.
        /// </summary>
        /// <param name="name"></param>
        public void SetFilterControlName(string name)
        {
            this.FilterControlName = name;
        }

        #region IXmlSerializable
        /// <summary>
        /// Standard method of the <see cref="IXmlSerializable"/> interface.
        /// </summary>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Standard method of the <see cref="IXmlSerializable"/> interface.
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("ColumnItemViewModel");
                reader.MoveToContent();
                while (reader.NodeType == System.Xml.XmlNodeType.Whitespace)
                    reader.Read();

                IsPropertyChangedEventEnabled = ReadTag<bool>(reader, "IsPropertyChangedEventEnabled");
                Header = ReadTag<string>(reader, "Header");
                Field = ReadTag<string>(reader, "Field");
                FilterControlName = ReadTag<string>(reader, "FilterControlName");
                StringFormat = ReadTag<string>(reader, "StringFormat");
                MinWidth = ReadTag<double>(reader, "MinWidth");
                Width = ReadTag<double>(reader, "Width");

                var strAlignment = ReadTag<string>(reader, "Alignment");
                Alignment = (CellAlignment)Enum.Parse(typeof(CellAlignment), strAlignment);

                IsColumnVisible = ReadTag<bool>(reader, "IsColumnVisible");

                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Standard method of the <see cref="IXmlSerializable"/> interface.
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            WriteElement(writer, "IsPropertyChangedEventEnabled", IsPropertyChangedEventEnabled);
            WriteElement(writer, "Header", Header);
            WriteElement(writer, "Field", Field);
            WriteElement(writer, "FilterControlName", FilterControlName);
            WriteElement(writer, "StringFormat", StringFormat);
            WriteElement(writer, "MinWidth", MinWidth);
            WriteElement(writer, "Width", Width);
            WriteElement(writer, "Alignment", Alignment);
            WriteElement(writer, "IsColumnVisible", IsColumnVisible);
        }

        /// <summary>
        /// Reads an XML Tag and returns its content in a type safe mode.
        /// 
        /// Optional tags (<paramref name="isOptional"/>) or empty tags are
        /// returned with default value (eg. null) - callers have to check
        /// if default is fine or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="tagName"></param>
        /// <param name="isOptional">On true: A <see cref="XmlException"/>is raised if
        /// the corresponding tag is empty or was not present.
        /// 
        /// On false: A default value is returned in both cases.</param>
        /// <returns></returns>
        private T ReadTag<T>(XmlReader reader,
                             string tagName,
                             bool isOptional = true)
        {
            // Tag is emtpy - so we consume it and return default value
            // Caller must check if default is applicable or not
            if (reader.IsEmptyElement == true)
            {
                if (isOptional == true)
                {
                    reader.Read();
                    return default(T);
                }
                else
                    throw new XmlException(string.Format("The tag '{0}' is empty.", tagName));
            }

            // Tag may not be available here - so we return with default
            if (reader.Name != tagName)
            {
                if (isOptional == true)
                    return default(T);
                else
                    throw new XmlException(string.Format("The tag '{0}' was not found at the expected location.", tagName));
            }

            reader.ReadStartElement(tagName);
            reader.MoveToContent();

            while (reader.NodeType == System.Xml.XmlNodeType.Whitespace)
                reader.Read();

            T ret = (T)reader.ReadContentAs(typeof(T), null);
            reader.ReadEndElement();

            return ret;
        }

        /// <summary>
        /// Writes the value of an object as XML tag with content into the
        /// given <see cref="XmlWriter"/> output parameter. Writes an empty
        /// tag if value appears to be null.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteElement(XmlWriter writer, string name, object value)
        {
            if (value != null)
            {
                if (value is bool)
                {
                    // bool implementation is inconsistent since it will write out 'True'
                    // but expects 'true' when casting from bool string to bool :-(
                    // https://stackoverflow.com/questions/491334/why-does-boolean-tostring-output-true-and-not-true
                    writer.WriteElementString(name, XmlConvert.ToString((bool)value));
                }
                else
                {
                    writer.WriteElementString(name,
                        string.Format(CultureInfo.InvariantCulture, "{0}", value));
                }
            }
            else
                writer.WriteElementString(name, string.Empty);
        }
        #endregion IXmlSerializable
        #endregion methods
    }
}