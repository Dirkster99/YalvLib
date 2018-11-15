using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using YalvLib.ViewModels.Common;

namespace YALV.View.Components
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextBox.xaml
    /// </summary>    
    public partial class AutoCompleteTextBox : Canvas
    {
        #region Members

        //private readonly ObservableCollection<AutoCompleteEntry> _autoCompletionList;
        private readonly ComboBox _comboBox;
        private readonly VisualCollection _controls;
        private readonly Timer _keypressTimer;
        private readonly TextBox _textBox;   

        public static readonly DependencyProperty AutoCompletionListProperty =
       DependencyProperty.Register("AutoCompleteList", typeof(ObservableCollection<AutoCompleteEntry>), typeof(AutoCompleteTextBox), new UIPropertyMetadata());


        private int _delayTime;
        private bool _insertText;
        private int _searchThreshold;

        private delegate void TextChangedCallback();

        #endregion

        #region Constructor

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
        }


        public AutoCompleteTextBox()
        {
            _controls = new VisualCollection(this);
            InitializeComponent();

            _searchThreshold = 2; // default threshold to 2 char

            // set up the key press timer
            _keypressTimer = new Timer();
            _keypressTimer.Elapsed += OnTimedEvent;

            // set up the text box and the combo box
            _comboBox = new ComboBox();
            _comboBox.IsSynchronizedWithCurrentItem = true;
            _comboBox.IsTabStop = false;
            _comboBox.SelectionChanged += comboBox_SelectionChanged;

            _textBox = new TextBox();
            _textBox.TextChanged += textBox_TextChanged;
            _textBox.VerticalContentAlignment = VerticalAlignment.Center;

            _controls.Add(_comboBox);
            _controls.Add(_textBox);
        }

        #endregion

        #region Methods

        public ObservableCollection<AutoCompleteEntry> AutoCompleteList
        {
            get { return (ObservableCollection<AutoCompleteEntry>)GetValue(AutoCompletionListProperty); }
            set { SetValue(AutoCompletionListProperty, value); }
        }


        public string Text
        {
            get { return _textBox.Text; }
            set
            {
                _insertText = true;
                _textBox.Text = value; 
            }
        }

        public int DelayTime
        {
            get { return _delayTime; }
            set { _delayTime = value; }
        }

        public int Threshold
        {
            get { return _searchThreshold; }
            set { _searchThreshold = value; }
        }

        protected override int VisualChildrenCount
        {
            get { return _controls.Count; }
        }

        public void AddItem(AutoCompleteEntry entry)
        {
            AutoCompleteList.Add(entry);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != _comboBox.SelectedItem)
            {
                _insertText = true;
                var cbItem = (ComboBoxItem) _comboBox.SelectedItem;
                _textBox.Text = cbItem.Content.ToString();
            }
        }

        public new Brush Background
        {
            get { return _textBox.Background; }
            set { _textBox.Background = value; }
        }

        private void TextChanged()
        {
            try
            {
                _comboBox.Items.Clear();
                if (_textBox.Text.Length >= _searchThreshold)
                {
                    foreach (AutoCompleteEntry entry in AutoCompleteList)
                    {
                        foreach (string word in entry.KeywordStrings)
                        {
                            if (word.StartsWith(_textBox.Text, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var cbItem = new ComboBoxItem();
                                cbItem.Content = entry.ToString();
                                _comboBox.Items.Add(cbItem);
                                break;
                            }
                        }
                    }
                    _comboBox.IsDropDownOpen = _comboBox.HasItems;
                }
                else
                {
                    _comboBox.IsDropDownOpen = false;
                }
            }
            catch
            {
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _keypressTimer.Stop();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                   new TextChangedCallback(TextChanged));
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // text was not typed, do nothing and consume the flag
            if (_insertText) _insertText = false;

                // if the delay time is set, delay handling of text changed
            else
            {
                if (_delayTime > 0)
                {
                    _keypressTimer.Interval = _delayTime;
                    _keypressTimer.Start();
                }
                else TextChanged();
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _textBox.Arrange(new Rect(arrangeSize));
            _comboBox.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _controls[index];
        }

        #endregion
    }
}