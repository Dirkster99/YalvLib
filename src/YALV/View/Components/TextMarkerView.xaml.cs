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

        private TextMarkerViewModel ViewModel
        {
            get { return DataContext as TextMarkerViewModel; }
        }

        public string AuthorInputValue
        {
            get { return this.AuthorTextBox.Text; }
        }

        private void DeleteMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(YalvLib.Strings.Resources.MarkerRow_DeleteConfirmation,
                            YalvLib.Strings.Resources.MarkerRow_DeleteConfirmation_Caption, MessageBoxButton.YesNo,
                            MessageBoxImage.Warning, MessageBoxResult.No);
            if(result == MessageBoxResult.Yes)
            {
                ((TextMarkerViewModel)this.DataContext).CommandCancelTextMarker.Execute(null);
            }
        }
    }
}
