namespace YalvLib.ViewModels
{
    using System.Globalization;

    /// <summary>
    /// Helper method to support culture based binding displays in XAML.
    /// </summary>
    public static class ResolveCultures
    {
        /// <summary>
        /// Gets the CultureInfo (similar to en-US) object for the currently configured culture.
        /// 
        /// This is used to evaluate culture based convertersion in XAML bindings.
        /// </summary>
        public static CultureInfo ResolvedCulture
        {
            get { return CultureInfo.GetCultureInfo(log4netLib.Strings.Resources.CultureName); }
        }
    }
}
