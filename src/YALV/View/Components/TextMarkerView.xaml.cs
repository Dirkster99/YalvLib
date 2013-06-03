using System;
using System.Windows;
using System.Windows.Controls;
using YalvLib.ViewModel;

namespace YALV.View.Components
{
    /// <summary>
    /// Interaction logic for TextMarkerView.xaml
    /// </summary>
    public partial class TextMarkerView : UserControl
    {
        public TextMarkerView()
        {
            InitializeComponent();
        }

        public string AuthorInputValue
        {
            get { return this.AuthorTextBox.Text; }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            if (binding != null) binding.UpdateSource();
        }



        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (((TextMarkerViewModel)DataContext).CommandChangeTextMarker.CanExecute(null))
            {
                ((TextMarkerViewModel)DataContext).CommandChangeTextMarker.Execute(null);
            }
        }
    }
}
