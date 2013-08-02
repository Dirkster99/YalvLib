using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    class AbstractMarkerMapping : ClassMap<AbstractMarker>
    {
        public AbstractMarkerMapping()
        {
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.LogEntries);
            Map(x => x.DateCreation);
            Map(x => x.DateLastModification);
        }

    }
}
