namespace EditableListLib.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Can be used to define the minimal and/or maximum number of lines
    /// that should be available for a <see cref="TextBlock"/> display.
    /// 
    /// Source: https://stackoverflow.com/questions/13637814/maximum-number-of-lines-for-a-wrap-textblock
    /// </summary>
    public static class TextBlockLinesBehavior
    {
        #region fields
        /// <summary>
        /// Backing store of the attached <see cref="MaxLinesProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.RegisterAttached("MaxLines", typeof(int),
                typeof(TextBlockLinesBehavior),
                new PropertyMetadata(default(int), OnMaxLinesPropertyChangedCallback));

        /// <summary>
        /// Backing store of the attached <see cref="MinLinesProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinLinesProperty =
            DependencyProperty.RegisterAttached("MinLines", typeof(int),
                typeof(TextBlockLinesBehavior), new PropertyMetadata(default(int), OnMinLinesPropertyChangedCallback));
        #endregion fields

        #region properties
        public static int GetMaxLines(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLinesProperty);
        }

        public static void SetMaxLines(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLinesProperty, value);
        }

        public static int GetMinLines(DependencyObject obj)
        {
            return (int)obj.GetValue(MinLinesProperty);
        }

        public static void SetMinLines(DependencyObject obj, int value)
        {
            obj.SetValue(MinLinesProperty, value);
        }
        #endregion properties

        #region methods
        private static void OnMaxLinesPropertyChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            TextBlock element = d as TextBlock;

            if (element != null)
                element.MaxHeight = getLineHeight(element) * GetMaxLines(element);
        }

        private static void OnMinLinesPropertyChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            TextBlock element = d as TextBlock;

            if (element != null)
                element.MinHeight = getLineHeight(element) * GetMinLines(element);
        }

        private static double getLineHeight(TextBlock textBlock)
        {
            double lineHeight = textBlock.LineHeight;

            if (double.IsNaN(lineHeight))
                lineHeight = Math.Ceiling(textBlock.FontSize * textBlock.FontFamily.LineSpacing);

            return lineHeight;
        }
        #endregion methods
    }
}
