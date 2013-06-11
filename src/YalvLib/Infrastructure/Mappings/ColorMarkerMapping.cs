using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    class ColorMarkerMapping : ClassMap<ColorMarker>
    {
        public ColorMarkerMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.LogEntries).Cascade.All();
            Map(x => x.HighlightColor);
            Map(x => x.DateCreation);
            Map(x => x.DateLastModification);
        }
    }
}
