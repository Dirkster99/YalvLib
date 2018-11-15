namespace YALV.View.Components
{
    using System.Windows;
    using System.Windows.Controls;
    using YalvLib.ViewModels;

    /// <summary>
    /// Interaction logic for TextMarkerView.xaml
    /// </summary>
    public partial class TextMarkerView : UserControl
    {
        public TextMarkerView()
        {
            InitializeComponent();
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
