namespace YalvLib.Infrastructure
{
    using log4netLib.Enums;
    using System;

    /// <summary>
    /// Implements a class that converts a string based level indication
    /// into a LevelIndex enum based value.
    /// </summary>
    public class LevelConverter
    {
        /// <summary>
        /// Static method to convert a string based level indication
        /// into a LevelIndex enum based value.
        /// </summary>
        /// <param name="level"></param>
        public static LevelIndex From(String level)
        {
            String ul = !string.IsNullOrWhiteSpace(level) ? level.Trim().ToUpper() : String.Empty;
            LevelIndex levelIndexParsed;
            try
            {
                Enum.TryParse(ul, true, out levelIndexParsed);
            }
            catch
            {
                levelIndexParsed = LevelIndex.NONE;
            }
            return levelIndexParsed;
        }
    }
}
