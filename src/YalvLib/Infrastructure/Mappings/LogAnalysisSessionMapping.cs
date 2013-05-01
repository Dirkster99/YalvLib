using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{

    public class LogAnalysisSessionMapping : ClassMap<LogAnalysisSession>
    {

        public LogAnalysisSessionMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.SourceRepositories).Cascade.All();
        }

    }

}
