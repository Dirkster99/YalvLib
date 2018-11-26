namespace EditableListLib.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    public static class TextBoxSelectAllOnFocus
    {
        // Using a DependencyProperty as the backing store for IsSelectAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectAllProperty =
            DependencyProperty.RegisterAttached("IsSelectAll", typeof(bool?),
                typeof(TextBoxSelectAllOnFocus), new PropertyMetadata(null, OnChanged));

        public static bool GetIsSelectAll(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectAllProperty);
        }

        public static void SetIsSelectAll(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectAllProperty, value);
        }

        private static void OnChanged(DependencyObject d,
                                      DependencyPropertyChangedEventArgs e)
        {
            var control = d as TextBox;

            if (control == null)
                return;

            if (e.OldValue == null && e.NewValue != null)
            {
                // Attach an event handler to handle this event
                control.GotFocus += Control_GotFocus;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                // Clean up if this is no longer wanted
                control.GotFocus += Control_GotFocus;
            }
        }

        private static void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;

            if (control == null)
                return;

            bool IsSelectAll = TextBoxSelectAllOnFocus.GetIsSelectAll(control);

            if (IsSelectAll == true)
                control.SelectAll();
        }
    }
}
