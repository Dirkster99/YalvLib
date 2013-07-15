using System.Windows;
using System.Windows.Controls;

namespace YalvLib.View
{
    /// <summary>
    /// DataGrid Behavior class
    /// This class implements an attached behaviour to scroll to an
    /// item that can programatically be selected via ViewModel binding
    /// </summary>
    public static class SelectedItem
    {
        #region IsBroughtIntoViewWhenSelected
        /// <summary>
        /// Determins if the DatGridItem is to be brought into view when enabled
        /// </summary>
        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "IsBroughtIntoViewWhenSelected",
            typeof(bool),
            typeof(SelectedItem),
            new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        /// <summary>
        /// Gets the IsBroughtIntoViewWhenSelected value
        /// </summary>
        /// <param name="listBoxItem"></param>
        /// <returns></returns>
        public static bool GetIsBroughtIntoViewWhenSelected(DataGrid listBoxItem)
        {
            return (bool)listBoxItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        /// <summary>
        /// Sets the IsBroughtIntoViewWhenSelected value
        /// </summary>
        /// <param name="listBoxItem"></param>
        /// <param name="value"></param>
        public static void SetIsBroughtIntoViewWhenSelected(
          DataGrid listBoxItem, bool value)
        {
            listBoxItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        /// <summary>
        /// Action to take when item is brought into view
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnIsBroughtIntoViewWhenSelectedChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid item = depObj as DataGrid;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.SelectionChanged += OnListViewItemSelected;
            else
                item.SelectionChanged -= OnListViewItemSelected;
        }

        private static void OnListViewItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the ListViewItem
            // whose IsSelected property was modified.  Ignore all ancestors 
            // who are merely reporting that a descendant's Selected fired. 
            if (!object.ReferenceEquals(sender, e.OriginalSource))
                return;

            DataGrid lv = e.OriginalSource as DataGrid;
            if (lv != null)
            {
                ////lv.SelectedItem = lv.LogEntryRowViewModels.GetItemAt(lv.LogEntryRowViewModels.Count - 1);
                if (lv.SelectedItem != null)
                {
                    lv.ScrollIntoView(lv.SelectedItem);
                    DataGridRow item = lv.ItemContainerGenerator.ContainerFromItem(lv.SelectedItem) as DataGridRow;

                    if (item != null)
                        item.Focus();
                }
            }
        }
        #endregion // IsBroughtIntoViewWhenSelected
    }
}
