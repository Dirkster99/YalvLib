using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{

    public class LogEntryFileRepositoryMapping : ClassMap<LogEntryFileRepository>
    {

        public LogEntryFileRepositoryMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.LogEntries)
                .Cascade.All()
                .Not.LazyLoad();
            Map(x => x.Path);
        }

    }

}
