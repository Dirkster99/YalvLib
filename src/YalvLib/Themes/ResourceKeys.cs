namespace YalvLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys      

        /// <summary>
        /// Gets the color key for the normal control enabled background color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");


        /// <summary>
        /// Gets a the applicable foreground Brush key that should be used for coloring text.
        /// </summary>
        public static readonly ComponentResourceKey ControlTextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlTextBrushKey");

        /// <summary>
        /// Gets the Brush key for the normal control enabled foreground color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");

        /// <summary>
        /// Gets the Brush key of the border color of a control.
        /// </summary>
        public static readonly ComponentResourceKey ControlBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlBorderBrushKey");

        /// <summary>
        /// Gets the Brush key of the border color of a control.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderBorderBrushKey");
    }
}
