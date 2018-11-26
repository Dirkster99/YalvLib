namespace EditableListLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Resource key management class to keep track of all resources
    /// that can be re-styled in applications that make use of the implemented controls.
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
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedBackgroundBrushKey");

        #region Item Not Focused
        /// <summary>
        /// Gets the base color key to color the background and border of the selected item
        /// when it is not focused.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedNotFocusedColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedNotFocusedColorKey");

        /// <summary>
        /// Gets the Border Brush key to color the border of the selected item
        /// when it is not focused.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedNotFocusedBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedNotFocusedBorderBrushKey");

        /// <summary>
        /// Gets the Background Brush key to color the background of the selected item
        /// when it is not focused.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedNotFocusedBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedNotFocusedBackgroundBrushKey");
        #endregion Item Not Focused

        #region MouseOver
        /// <summary>
        /// Gets the Border Brush key to color of an item
        /// when the mouse cursor is over it.
        /// </summary>
        public static readonly ComponentResourceKey ItemMouseOverBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemMouseOverBorderBrushKey");

        /// <summary>
        /// Gets the Border Brush key to color of an item
        /// when the mouse cursor is over it.
        /// </summary>
        public static readonly ComponentResourceKey ItemMouseOverBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemMouseOverBackgroundBrushKey");
        #endregion MouseOver

        public static readonly ComponentResourceKey ItemSeperatorBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSeperatorBackgroundBrushKey");
    }
}
