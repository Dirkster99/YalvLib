namespace EditableListLib.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements an attached textbox behavior to convert ALT+Enter or Ctrl+Enter
    /// into an Enter character being added into the text.
    /// 
    /// (Using the behavior enables the single Enter key to be used as end of text input
    /// if you set the <see cref="TextBox"/> property AcceptReturn to false).
    /// 
    /// Based on:
    /// https://stackoverflow.com/questions/8139420/textbox-convert-sent-key-alt-enter-enter
    /// </summary>
    public static class TextBoxKeyPressBehavior
    {
        /// <summary>
        /// Backing store of the attached <see cref="TranslateEnterProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TranslateEnterProperty =
            DependencyProperty.RegisterAttached("TranslateEnter", typeof(bool?),
                typeof(TextBoxKeyPressBehavior), new PropertyMetadata(null,
                    OnTranslateEnterChanged));

        /// <summary>
        /// Gets the value of the attached <see cref="TranslateEnterProperty"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetTranslateEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(TranslateEnterProperty);
        }

        /// <summary>
        /// Sets the value of the attached <see cref="TranslateEnterProperty"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTranslateEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(TranslateEnterProperty, value);
        }

        /// <summary>
        /// Executes when a <see cref="TranslateEnterProperty"/> has been changed which
        /// occurs typically on attach and de-tach of this property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void OnTranslateEnterChanged(
            DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBox)
            {
                TextBox tb = obj as TextBox;

                if (e.OldValue == null && e.NewValue != null)
                {
                    // Attach an event handler to handle keyboard translation
                    tb.PreviewKeyDown += Tb_PreviewKeyDown;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    // Clean up if translation is no longer wanted
                    tb.PreviewKeyDown -= Tb_PreviewKeyDown;
                }
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of a <see cref="TextBox"/> to translate
        /// ALT+ENTER or CTRL+ENTER into an enter character being added into the current
        /// text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Tb_PreviewKeyDown(object sender,
                                              System.Windows.Input.KeyEventArgs e)
        {
            var tb = (sender as TextBox);

            // Handle Alt + Enter and Ctrl + Enter as Enter
            if ((Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter)) ||
                (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.Enter)))
            {
                // Is text currently selected - then replace it with enter character
                if (string.IsNullOrEmpty(tb.SelectedText) == false)
                {
                    tb.SelectedText = "\r\n";
                    tb.SelectionStart = tb.Text.Length;
                    e.Handled = true;
                }
                else
                {
                    // Just Add the enter character to the available text
                    tb.Text += "\r\n";
                    tb.SelectionStart = tb.Text.Length;
                    e.Handled = true;
                }
            }
        }
    }
}
