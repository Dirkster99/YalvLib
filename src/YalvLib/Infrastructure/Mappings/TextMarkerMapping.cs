using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    public class TextMarkerMapping : ClassMap<TextMarker>
    {
        public TextMarkerMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.LogEntries).Cascade.All().Not.LazyLoad();
            Map(x => x.Author);
            Map(x => x.Message);
            Map(x => x.DateCreation);
            Map(x => x.DateLastModification);
        }
    }
}
