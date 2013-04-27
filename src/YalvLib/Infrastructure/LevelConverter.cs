using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.Infrastructure
{

    public class LevelConverter
    {

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
