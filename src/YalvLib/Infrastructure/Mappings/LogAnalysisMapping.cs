using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{
    public class LogAnalysisMapping: ClassMap<LogAnalysis>
    {

        public LogAnalysisMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();

            HasMany(x => x.TextMarkers).Not.LazyLoad()
                .Cascade.All();
        }

    }
}
