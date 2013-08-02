using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    public class TextMarkerMapping : SubclassMap<TextMarker>
    {
        public TextMarkerMapping()
        {
            Not.LazyLoad();
            Map(x => x.Author);
            Map(x => x.Message);
        }
    }
}
