using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YalvLib.ViewModel;

namespace YalvLib.View
{
    public class MarkerTemplateSelector : DataTemplateSelector
    {

        public DataTemplate TextAndColorMarkerTemplate
        {
            get; set; 
        }

        public DataTemplate ColorMarkerTemplate
        {
            get; set; 
        }

        public DataTemplate TextMarkerTemplate
        {
            get;
            set;
        }

        public DataTemplate NoMarkerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            /*DataTemplate txtMarker = dataGrid.FindResource("TextMarkerDataTemplate") as DataTemplate;
            DataTemplate colorMarker = dataGrid.FindResource("ColorMarkerDataTemplate") as DataTemplate;
            DataTemplate bothMarker = dataGrid.FindResource("TextAndColorMarkerDataTemplate") as DataTemplate;
            DataTemplate noMarker = dataGrid.FindResource("NoMarkerDataTemplate") as DataTemplate;*/

            
            LogEntryRowViewModel logEntryVM = item as LogEntryRowViewModel;

            if (logEntryVM == null)
                return null;

            if (logEntryVM.ColorMarkerQuantity >= 1 && logEntryVM.TextMarkerQuantity >= 1)
                return TextAndColorMarkerTemplate;

            if (logEntryVM.ColorMarkerQuantity >= 1 && logEntryVM.TextMarkerQuantity == 0)
                return ColorMarkerTemplate;

            if (logEntryVM.TextMarkerQuantity >= 1 && logEntryVM.ColorMarkerQuantity == 0)
                return TextMarkerTemplate;

            return NoMarkerTemplate;
        }

    }
}
