using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    public class ColorMarkerMapping : ClassMap<ColorMarker>
    {
        public ColorMarkerMapping()
        {
            Not.LazyLoad();
            Map(x => x.HighlightColor);
        }
    }
}
