namespace EditableListLib.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public static class IsKeyboardFocusWithinChanged
    {
        /// <summary>
        /// Backing store of the attached <see cref="IsKeyboardFocusWithinChangedProperty"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand), typeof(IsKeyboardFocusWithinChanged),
                new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if (element == null)
                return;

            if (e.OldValue == null && e.NewValue != null)
            {
                // Attach an event handler to handle this event
                element.IsKeyboardFocusWithinChanged += Panel_IsKeyboardFocusWithinChanged;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                // Clean up if this is no longer wanted
                element.IsKeyboardFocusWithinChanged -= Panel_IsKeyboardFocusWithinChanged;
            }
        }

        private static void Panel_IsKeyboardFocusWithinChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            var element = sender as UIElement;

            if (element == null)
                return;

            ICommand clickCommand = IsKeyboardFocusWithinChanged.GetCommand(element);

            if (clickCommand != null)
            {
                // Check whether this attached behaviour is bound to a RoutedCommand
                if (clickCommand is RoutedCommand)
                {
                    // Execute the routed command
                    (clickCommand as RoutedCommand).Execute(e.NewValue, element);
                }
                else
                {
                    // Execute the Command as bound delegate
                    clickCommand.Execute(e.NewValue);
                }
            }
        }
    }
}
