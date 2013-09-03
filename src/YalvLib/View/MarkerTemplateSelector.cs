using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YalvLib.ViewModel;

namespace YalvLib.View
{
    /// <summary>
    /// Marker Data Template Selector
    /// </summary>
    public class MarkerTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// TextAndColorMarker DataTemplate
        /// </summary>
        public DataTemplate TextAndColorMarkerTemplate
        {
            get; set; 
        }

        /// <summary>
        /// ColorMarker DataTemplate
        /// </summary>
        public DataTemplate ColorMarkerTemplate
        {
            get; set; 
        }

        /// <summary>
        /// TextMarker DataTemplate
        /// </summary>
        public DataTemplate TextMarkerTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// NoMarker DataTemplate
        /// </summary>
        public DataTemplate NoMarkerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {    
            var logEntryVm = item as LogEntryRowViewModel;

            if (logEntryVm == null)
                return null;

            if (logEntryVm.ColorMarkerQuantity >= 1 && logEntryVm.TextMarkerQuantity >= 1)
                return TextAndColorMarkerTemplate;

            if (logEntryVm.ColorMarkerQuantity >= 1 && logEntryVm.TextMarkerQuantity == 0)
                return ColorMarkerTemplate;

            if (logEntryVm.TextMarkerQuantity >= 1 && logEntryVm.ColorMarkerQuantity == 0)
                return TextMarkerTemplate;

            return NoMarkerTemplate;
        }

    }
}
