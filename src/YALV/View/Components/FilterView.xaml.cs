using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YalvLib.ViewModel;
using YalvLib.ViewModel.Common;

namespace YALV.View.Components
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl
    {
        public FilterView()
        {
            InitializeComponent();          
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (((DisplayLogViewModel)DataContext).CommandApplyFilter.CanExecute(textBox_Filter.Text))
            {
                textBox_Filter.Background = Brushes.White;
                ((DisplayLogViewModel)DataContext).CommandApplyFilter.Execute(null);
            }else
            {
                textBox_Filter.Background = Brushes.IndianRed;
            }
        }

        private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
        {
            textBox_Filter.Text = "";
        }
    }
}
